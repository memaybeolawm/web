// ============================================================
// store.js – Asynchronous API Client with Customer & Admin Auth
// ============================================================

const Store = (() => {
  const API_BASE = '/api';
  const CART_KEY = 'hc_cart';
  const USER_KEY = 'hc_current_user';
  const ADMIN_KEY = 'hc_current_admin';

  const ORDER_STATUSES = {
    PENDING:   { label: 'Chờ xác nhận', color: '#F59E0B', icon: '⏳' },
    CONFIRMED: { label: 'Đã xác nhận',  color: '#3B82F6', icon: '✅' },
    PREPARING: { label: 'Đang chuẩn bị',color: '#8B5CF6', icon: '📦' },
    SHIPPING:  { label: 'Đang giao',    color: '#EC4899', icon: '🚚' },
    COMPLETED: { label: 'Hoàn thành',   color: '#10B981', icon: '🎉' },
    CANCELLED: { label: 'Đã hủy',       color: '#EF4444', icon: '❌' },
  };

  // ── Helper fetch wrapper ───────────────────────────────────
  async function request(endpoint, options = {}) {
    try {
      const response = await fetch(API_BASE + endpoint, {
        headers: { 'Content-Type': 'application/json', ...(options.headers || {}) },
        ...options,
      });
      const data = await response.json();
      if (!response.ok) {
        throw new Error(data.message || data.error || 'Lỗi hệ thống!');
      }
      return data;
    } catch (err) {
      console.error(`API Error [${endpoint}]:`, err);
      throw err;
    }
  }

  // ── Init ──────────────────────────────────────────────────
  async function init() {
    try {
      await request('/categories');
    } catch (e) {
      console.warn('API connection check failed:', e);
    }
  }

  // ── Customer Auth ──────────────────────────────────────────
  async function registerCustomer(userData) {
    try {
      const res = await request('/auth/register', {
        method: 'POST',
        body: JSON.stringify(userData),
      });
      if (res.success && res.user) {
        localStorage.setItem(USER_KEY, JSON.stringify(res.user));
      }
      return res;
    } catch (err) {
      return { success: false, message: err.message };
    }
  }

  async function loginCustomer(email, password) {
    try {
      const res = await request('/auth/login', {
        method: 'POST',
        body: JSON.stringify({ email, password }),
      });
      if (res.success && res.user) {
        localStorage.setItem(USER_KEY, JSON.stringify(res.user));
      }
      return res;
    } catch (err) {
      return { success: false, message: err.message };
    }
  }

  function logoutCustomer() {
    localStorage.removeItem(USER_KEY);
  }

  function getCurrentUser() {
    try {
      return JSON.parse(localStorage.getItem(USER_KEY)) || null;
    } catch (e) {
      return null;
    }
  }

  async function updateProfile(profileData) {
    try {
      const res = await request('/auth/profile', {
        method: 'PUT',
        body: JSON.stringify(profileData),
      });
      if (res.success && res.user) {
        localStorage.setItem(USER_KEY, JSON.stringify(res.user));
      }
      return res;
    } catch (err) {
      return { success: false, message: err.message };
    }
  }

  // ── Admin Auth ────────────────────────────────────────────
  async function loginAdmin(username, password) {
    try {
      const res = await request('/admin/login', {
        method: 'POST',
        body: JSON.stringify({ username, password }),
      });
      if (res.success && res.admin) {
        localStorage.setItem(ADMIN_KEY, JSON.stringify(res.admin));
      }
      return res;
    } catch (err) {
      return { success: false, message: err.message };
    }
  }

  function logoutAdmin() {
    localStorage.removeItem(ADMIN_KEY);
  }

  function getCurrentAdmin() {
    try {
      return JSON.parse(localStorage.getItem(ADMIN_KEY)) || null;
    } catch (e) {
      return null;
    }
  }

  function requireAdmin() {
    const admin = getCurrentAdmin();
    if (!admin) {
      window.location.href = 'login.html';
      return false;
    }
    return true;
  }

  // ── Categories ────────────────────────────────────────────
  async function getCategories() {
    return await request('/categories');
  }

  // ── Products ──────────────────────────────────────────────
  async function getProducts(params = {}) {
    const query = new URLSearchParams();
    if (params.cat) query.append('cat', params.cat);
    if (params.q) query.append('q', params.q);
    if (params.minPrice) query.append('minPrice', params.minPrice);
    if (params.maxPrice) query.append('maxPrice', params.maxPrice);
    if (params.sort) query.append('sort', params.sort);
    
    const url = '/products' + (query.toString() ? '?' + query.toString() : '');
    return await request(url);
  }

  async function getProduct(id) {
    return await request(`/products/${id}`);
  }

  async function saveProduct(product) {
    if (product.id && (await productExists(product.id))) {
      return await request(`/products/${product.id}`, {
        method: 'PUT',
        body: JSON.stringify(product),
      });
    } else {
      return await request('/products', {
        method: 'POST',
        body: JSON.stringify(product),
      });
    }
  }

  async function productExists(id) {
    try {
      await request(`/products/${id}`);
      return true;
    } catch (e) {
      return false;
    }
  }

  async function deleteProduct(id) {
    return await request(`/products/${id}`, { method: 'DELETE' });
  }

  // ── Cart (LocalStorage) ────────────────────────────────────
  function getCart() {
    try { return JSON.parse(localStorage.getItem(CART_KEY)) || []; }
    catch(e) { return []; }
  }

  function saveCart(cart) {
    localStorage.setItem(CART_KEY, JSON.stringify(cart));
  }

  function addToCart(productId, quantity = 1, productStock = 99) {
    const cart = getCart();
    const existing = cart.find(i => i.productId === productId);
    const currentQty = existing ? existing.quantity : 0;
    if (currentQty + quantity > productStock) {
      return { success: false, message: `Chỉ còn ${productStock} sản phẩm trong kho!` };
    }
    if (existing) existing.quantity += quantity;
    else cart.push({ productId, quantity });
    saveCart(cart);
    return { success: true };
  }

  function updateCartItem(productId, quantity) {
    const cart = getCart();
    const existing = cart.find(i => i.productId === productId);
    if (existing) {
      existing.quantity = Math.max(1, quantity);
      saveCart(cart);
    }
    return { success: true };
  }

  function removeFromCart(productId) {
    const cart = getCart().filter(i => i.productId !== productId);
    saveCart(cart);
  }

  function clearCart() {
    localStorage.removeItem(CART_KEY);
  }

  function getCartTotal(productsList = []) {
    const cart = getCart();
    return cart.reduce((sum, item) => {
      const p = productsList.find(p => p.id === item.productId);
      return sum + (p ? p.price * item.quantity : 0);
    }, 0);
  }

  function getCartCount() {
    return getCart().reduce((sum, i) => sum + i.quantity, 0);
  }

  // ── Orders ────────────────────────────────────────────────
  async function createOrder(customerInfo, cartItems, paymentMethod) {
    try {
      const currentUser = getCurrentUser();
      const userId = currentUser ? currentUser.id : null;
      const res = await request('/orders', {
        method: 'POST',
        body: JSON.stringify({ customerInfo, items: cartItems, paymentMethod, userId }),
      });
      if (res.success) clearCart();
      return res;
    } catch (err) {
      return { success: false, message: err.message };
    }
  }

  async function getOrders(params = {}) {
    const query = new URLSearchParams();
    if (params.status) query.append('status', params.status);
    if (params.q) query.append('q', params.q);
    if (params.userId) query.append('userId', params.userId);
    const url = '/orders' + (query.toString() ? '?' + query.toString() : '');
    return await request(url);
  }

  async function getOrder(id) {
    try {
      return await request(`/orders/${id}`);
    } catch (e) {
      return null;
    }
  }

  async function updateOrderStatus(orderId, status, note = '') {
    return await request(`/orders/${orderId}/status`, {
      method: 'PATCH',
      body: JSON.stringify({ status, note }),
    });
  }

  // ── Reviews ───────────────────────────────────────────────
  async function getReviews(productId = null) {
    const url = '/reviews' + (productId ? `?productId=${productId}` : '');
    return await request(url);
  }

  async function addReview(productId, orderId, customerName, rating, comment) {
    try {
      return await request('/reviews', {
        method: 'POST',
        body: JSON.stringify({ productId, orderId, customerName, rating, comment }),
      });
    } catch (err) {
      return { success: false, message: err.message };
    }
  }

  async function deleteReview(id) {
    return await request(`/reviews/${id}`, { method: 'DELETE' });
  }

  // ── Stats ─────────────────────────────────────────────────
  async function getStats() {
    return await request('/stats');
  }

  // ── Reset ─────────────────────────────────────────────────
  async function reset() {
    clearCart();
    logoutCustomer();
    logoutAdmin();
    return await request('/reset', { method: 'POST' });
  }

  // ── Formatting Utilities ──────────────────────────────────
  function formatPrice(amount) {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount || 0);
  }

  function formatDate(isoString) {
    if (!isoString) return '';
    const d = new Date(isoString);
    return d.toLocaleDateString('vi-VN', {
      hour: '2-digit', minute: '2-digit',
      day: '2-digit', month: '2-digit', year: 'numeric'
    });
  }

  function generateId(prefix = 'id') {
    return prefix + '-' + Math.random().toString(36).substr(2, 9);
  }

  return {
    ORDER_STATUSES,
    init,
    // Customer Auth
    registerCustomer,
    loginCustomer,
    logoutCustomer,
    getCurrentUser,
    updateProfile,
    // Admin Auth
    loginAdmin,
    logoutAdmin,
    getCurrentAdmin,
    requireAdmin,
    // Catalog & DB
    getCategories,
    getProducts,
    getProduct,
    saveProduct,
    deleteProduct,
    getCart,
    addToCart,
    updateCartItem,
    removeFromCart,
    clearCart,
    getCartTotal,
    getCartCount,
    createOrder,
    getOrders,
    getOrder,
    updateOrderStatus,
    getReviews,
    addReview,
    deleteReview,
    getStats,
    reset,
    formatPrice,
    formatDate,
    generateId,
  };
})();
