// ============================================================
// server.js – Express REST API Server with SQLite & Auth
// ============================================================

const express = require('express');
const cors = require('cors');
const path = require('path');
const db = require('./db');
const seedData = require('./js/data');

const app = express();
const PORT = process.env.PORT || 8888;

app.use(cors());
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

// Disable caching for CSS/JS to ensure fresh files are always served
app.use((req, res, next) => {
  if (req.url.match(/\.(css|js)(\?.*)?$/)) {
    res.setHeader('Cache-Control', 'no-store, no-cache, must-revalidate');
    res.setHeader('Pragma', 'no-cache');
  }
  next();
});

// Serve static frontend files
app.use(express.static(path.join(__dirname)));

// ── Database Schema Initialization ──────────────────────────
async function initDatabase() {
  await db.exec(`
    CREATE TABLE IF NOT EXISTS categories (
      id TEXT PRIMARY KEY,
      name TEXT NOT NULL,
      icon TEXT,
      description TEXT
    );

    CREATE TABLE IF NOT EXISTS products (
      id TEXT PRIMARY KEY,
      category_id TEXT NOT NULL,
      name TEXT NOT NULL,
      price INTEGER NOT NULL,
      stock INTEGER NOT NULL,
      sold INTEGER DEFAULT 0,
      description TEXT,
      tags TEXT,
      images TEXT,
      rating REAL DEFAULT 0,
      review_count INTEGER DEFAULT 0,
      created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
      FOREIGN KEY (category_id) REFERENCES categories(id)
    );

    CREATE TABLE IF NOT EXISTS users (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      email TEXT UNIQUE NOT NULL,
      password TEXT NOT NULL,
      name TEXT NOT NULL,
      phone TEXT,
      address TEXT,
      city TEXT,
      created_at DATETIME DEFAULT CURRENT_TIMESTAMP
    );

    CREATE TABLE IF NOT EXISTS admins (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      username TEXT UNIQUE NOT NULL,
      password TEXT NOT NULL,
      name TEXT NOT NULL,
      role TEXT DEFAULT 'admin',
      created_at DATETIME DEFAULT CURRENT_TIMESTAMP
    );

    CREATE TABLE IF NOT EXISTS orders (
      id TEXT PRIMARY KEY,
      user_id INTEGER,
      customer_name TEXT NOT NULL,
      customer_phone TEXT NOT NULL,
      customer_email TEXT,
      customer_address TEXT NOT NULL,
      customer_city TEXT NOT NULL,
      customer_district TEXT,
      customer_ward TEXT,
      customer_note TEXT,
      gift_wrap INTEGER DEFAULT 0,
      total INTEGER NOT NULL,
      payment_method TEXT NOT NULL,
      status TEXT DEFAULT 'pending',
      created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
      updated_at DATETIME DEFAULT CURRENT_TIMESTAMP,
      FOREIGN KEY (user_id) REFERENCES users(id)
    );

    CREATE TABLE IF NOT EXISTS order_items (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      order_id TEXT NOT NULL,
      product_id TEXT NOT NULL,
      product_name TEXT NOT NULL,
      product_image TEXT,
      price INTEGER NOT NULL,
      quantity INTEGER NOT NULL,
      FOREIGN KEY (order_id) REFERENCES orders(id) ON DELETE CASCADE
    );

    CREATE TABLE IF NOT EXISTS order_status_history (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      order_id TEXT NOT NULL,
      status TEXT NOT NULL,
      time DATETIME DEFAULT CURRENT_TIMESTAMP,
      note TEXT,
      FOREIGN KEY (order_id) REFERENCES orders(id) ON DELETE CASCADE
    );

    CREATE TABLE IF NOT EXISTS reviews (
      id TEXT PRIMARY KEY,
      product_id TEXT NOT NULL,
      order_id TEXT NOT NULL,
      customer_name TEXT NOT NULL,
      rating INTEGER NOT NULL,
      comment TEXT,
      created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
      FOREIGN KEY (product_id) REFERENCES products(id),
      FOREIGN KEY (order_id) REFERENCES orders(id)
    );
  `);

  // Migration: add user_id column to orders if table previously existed without it
  try {
    await db.exec('ALTER TABLE orders ADD COLUMN user_id INTEGER REFERENCES users(id)');
  } catch (e) {
    // Column already exists
  }

  // Seed default categories & products if empty
  const categoryCount = await db.get('SELECT COUNT(*) as count FROM categories');
  if (categoryCount.count === 0) {
    console.log('Seeding initial data into SQLite database...');
    await seedDatabase();
  }

  // Seed default admin if empty
  const adminCount = await db.get('SELECT COUNT(*) as count FROM admins');
  if (adminCount.count === 0) {
    await db.run(
      'INSERT INTO admins (username, password, name, role) VALUES (?, ?, ?, ?)',
      ['admin', 'admin123', 'Quản trị viên', 'admin']
    );
    console.log('Default Admin created: username=admin, password=admin123');
  }

  // Seed default customer if empty
  const userCount = await db.get('SELECT COUNT(*) as count FROM users');
  if (userCount.count === 0) {
    await db.run(
      'INSERT INTO users (email, password, name, phone, address, city) VALUES (?, ?, ?, ?, ?, ?)',
      ['khach@gmail.com', '123456', 'Nguyễn Thị Khách', '0987654321', '456 Lê Lợi, Phường 1', 'TP. Hồ Chí Minh']
    );
    console.log('Default Customer created: email=khach@gmail.com, password=123456');
  }
}

async function seedDatabase() {
  await db.transaction(async () => {
    // Categories
    for (const c of seedData.CATEGORIES) {
      await db.run(
        'INSERT INTO categories (id, name, icon, description) VALUES (?, ?, ?, ?)',
        [c.id, c.name, c.icon, c.description]
      );
    }

    // Products
    for (const p of seedData.PRODUCTS) {
      await db.run(
        `INSERT INTO products (id, category_id, name, price, stock, sold, description, tags, images, rating, review_count)
         VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)`,
        [
          p.id, p.categoryId, p.name, p.price, p.stock, p.sold || 0,
          p.description || '',
          JSON.stringify(p.tags || []),
          JSON.stringify(p.images || []),
          p.rating || 0,
          p.reviewCount || 0,
        ]
      );
    }

    // Demo Order for reviews
    const demoOrderId = 'demo-order';
    await db.run(
      `INSERT INTO orders (id, customer_name, customer_phone, customer_email, customer_address, customer_city, total, payment_method, status)
       VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)`,
      [demoOrderId, 'Khách hàng Demo', '0901234567', 'demo@handmade.vn', '123 Nguyễn Trãi', 'TP. Hồ Chí Minh', 500000, 'COD', 'completed']
    );

    // Reviews
    for (const r of seedData.SAMPLE_REVIEWS) {
      await db.run(
        `INSERT INTO reviews (id, product_id, order_id, customer_name, rating, comment, created_at)
         VALUES (?, ?, ?, ?, ?, ?, ?)`,
        [r.id, r.productId, r.orderId, r.customerName, r.rating, r.comment, r.createdAt || new Date().toISOString()]
      );
    }
  });
  console.log('Database seeded successfully!');
}

// ── AUTHENTICATION API ENDPOINTS ────────────────────────────

// ── 1. Customer Auth API ────────────────────────────────────
app.post('/api/auth/register', async (req, res) => {
  try {
    const { email, password, name, phone, address, city } = req.body;
    if (!email || !password || !name) {
      return res.status(400).json({ success: false, message: 'Vui lòng nhập Email, Mật khẩu và Họ tên!' });
    }

    const existing = await db.get('SELECT * FROM users WHERE email = ?', [email.trim().toLowerCase()]);
    if (existing) {
      return res.status(400).json({ success: false, message: 'Email này đã được đăng ký tài khoản!' });
    }

    const result = await db.run(
      'INSERT INTO users (email, password, name, phone, address, city) VALUES (?, ?, ?, ?, ?, ?)',
      [email.trim().toLowerCase(), password, name, phone || '', address || '', city || '']
    );

    const user = await db.get('SELECT id, email, name, phone, address, city, created_at FROM users WHERE id = ?', [result.lastID]);
    res.json({ success: true, message: 'Đăng ký tài khoản thành công!', user });
  } catch (err) {
    res.status(500).json({ success: false, message: err.message });
  }
});

app.post('/api/auth/login', async (req, res) => {
  try {
    const { email, password } = req.body;
    if (!email || !password) {
      return res.status(400).json({ success: false, message: 'Nhập email và mật khẩu!' });
    }

    const user = await db.get(
      'SELECT id, email, name, phone, address, city, created_at FROM users WHERE email = ? AND password = ?',
      [email.trim().toLowerCase(), password]
    );

    if (!user) {
      return res.status(401).json({ success: false, message: 'Email hoặc mật khẩu không chính xác!' });
    }

    res.json({ success: true, message: 'Đăng nhập thành công!', user });
  } catch (err) {
    res.status(500).json({ success: false, message: err.message });
  }
});

app.put('/api/auth/profile', async (req, res) => {
  try {
    const { id, name, phone, address, city } = req.body;
    await db.run(
      'UPDATE users SET name = ?, phone = ?, address = ?, city = ? WHERE id = ?',
      [name, phone, address, city, id]
    );
    const user = await db.get('SELECT id, email, name, phone, address, city, created_at FROM users WHERE id = ?', [id]);
    res.json({ success: true, user });
  } catch (err) {
    res.status(500).json({ success: false, message: err.message });
  }
});

// ── 2. Admin Auth API ───────────────────────────────────────
app.post('/api/admin/login', async (req, res) => {
  try {
    const { username, password } = req.body;
    if (!username || !password) {
      return res.status(400).json({ success: false, message: 'Nhập tên đăng nhập và mật khẩu!' });
    }

    const admin = await db.get(
      'SELECT id, username, name, role FROM admins WHERE username = ? AND password = ?',
      [username.trim(), password]
    );

    if (!admin) {
      return res.status(401).json({ success: false, message: 'Tài khoản hoặc mật khẩu Admin không đúng!' });
    }

    const token = 'admin-token-' + Date.now();
    res.json({ success: true, message: 'Đăng nhập Admin thành công!', admin, token });
  } catch (err) {
    res.status(500).json({ success: false, message: err.message });
  }
});

// ── REST API ENDPOINTS FOR DATA ─────────────────────────────

// ── Categories
app.get('/api/categories', async (req, res) => {
  try {
    const categories = await db.all('SELECT * FROM categories');
    res.json(categories);
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

// ── Products
app.get('/api/products', async (req, res) => {
  try {
    const { cat, q, minPrice, maxPrice, sort } = req.query;
    let sql = 'SELECT * FROM products WHERE 1=1';
    const params = [];

    if (cat) {
      sql += ' AND category_id = ?';
      params.push(cat);
    }

    if (q) {
      sql += ' AND (LOWER(name) LIKE ? OR LOWER(description) LIKE ? OR LOWER(tags) LIKE ?)';
      const searchTerm = `%${q.toLowerCase()}%`;
      params.push(searchTerm, searchTerm, searchTerm);
    }

    if (minPrice) {
      sql += ' AND price >= ?';
      params.push(parseInt(minPrice));
    }

    if (maxPrice) {
      sql += ' AND price <= ?';
      params.push(parseInt(maxPrice));
    }

    switch (sort) {
      case 'price-asc':  sql += ' ORDER BY price ASC'; break;
      case 'price-desc': sql += ' ORDER BY price DESC'; break;
      case 'popular':    sql += ' ORDER BY sold DESC'; break;
      case 'rating':     sql += ' ORDER BY rating DESC'; break;
      default:           sql += ' ORDER BY created_at DESC'; break;
    }

    const rows = await db.all(sql, params);
    const products = rows.map(p => ({
      id: p.id,
      categoryId: p.category_id,
      name: p.name,
      price: p.price,
      stock: p.stock,
      sold: p.sold,
      description: p.description,
      tags: JSON.parse(p.tags || '[]'),
      images: JSON.parse(p.images || '[]'),
      rating: p.rating,
      reviewCount: p.review_count,
      createdAt: p.created_at,
    }));

    res.json(products);
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

app.get('/api/products/:id', async (req, res) => {
  try {
    const p = await db.get('SELECT * FROM products WHERE id = ?', [req.params.id]);
    if (!p) return res.status(404).json({ error: 'Sản phẩm không tồn tại!' });

    res.json({
      id: p.id,
      categoryId: p.category_id,
      name: p.name,
      price: p.price,
      stock: p.stock,
      sold: p.sold,
      description: p.description,
      tags: JSON.parse(p.tags || '[]'),
      images: JSON.parse(p.images || '[]'),
      rating: p.rating,
      reviewCount: p.review_count,
      createdAt: p.created_at,
    });
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

app.post('/api/products', async (req, res) => {
  try {
    const { id, categoryId, name, price, stock, description, tags, images } = req.body;
    const productId = id || ('prd-' + Date.now());

    await db.run(
      `INSERT INTO products (id, category_id, name, price, stock, sold, description, tags, images, rating, review_count)
       VALUES (?, ?, ?, ?, ?, 0, ?, ?, ?, 0, 0)`,
      [
        productId, categoryId, name, price, stock,
        description || '',
        JSON.stringify(tags || []),
        JSON.stringify(images || []),
      ]
    );

    res.json({ success: true, id: productId });
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

app.put('/api/products/:id', async (req, res) => {
  try {
    const { categoryId, name, price, stock, description, tags, images } = req.body;
    await db.run(
      `UPDATE products 
       SET category_id = ?, name = ?, price = ?, stock = ?, description = ?, tags = ?, images = ?
       WHERE id = ?`,
      [
        categoryId, name, price, stock,
        description || '',
        JSON.stringify(tags || []),
        JSON.stringify(images || []),
        req.params.id,
      ]
    );
    res.json({ success: true });
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

app.delete('/api/products/:id', async (req, res) => {
  try {
    await db.run('DELETE FROM products WHERE id = ?', [req.params.id]);
    res.json({ success: true });
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

// ── Orders
app.get('/api/orders', async (req, res) => {
  try {
    const { status, q, userId } = req.query;
    let sql = 'SELECT * FROM orders WHERE 1=1';
    const params = [];

    if (userId) {
      sql += ' AND user_id = ?';
      params.push(userId);
    }

    if (status && status !== 'all') {
      sql += ' AND status = ?';
      params.push(status);
    }

    if (q) {
      sql += ' AND (LOWER(id) LIKE ? OR LOWER(customer_name) LIKE ? OR customer_phone LIKE ?)';
      const term = `%${q.toLowerCase()}%`;
      params.push(term, term, term);
    }

    sql += ' ORDER BY created_at DESC';
    const orders = await db.all(sql, params);

    const result = [];
    for (const o of orders) {
      const items = await db.all('SELECT * FROM order_items WHERE order_id = ?', [o.id]);
      const history = await db.all('SELECT * FROM order_status_history WHERE order_id = ? ORDER BY time ASC', [o.id]);

      result.push({
        id: o.id,
        userId: o.user_id,
        customer: {
          name: o.customer_name,
          phone: o.customer_phone,
          email: o.customer_email,
          address: o.customer_address,
          city: o.customer_city,
          district: o.customer_district,
          ward: o.customer_ward,
          note: o.customer_note,
          giftWrap: !!o.gift_wrap,
        },
        items: items.map(i => ({
          productId: i.product_id,
          productName: i.product_name,
          productImage: i.product_image,
          price: i.price,
          quantity: i.quantity,
        })),
        total: o.total,
        paymentMethod: o.payment_method,
        status: o.status,
        createdAt: o.created_at,
        updatedAt: o.updated_at,
        statusHistory: history.map(h => ({ status: h.status, time: h.time, note: h.note })),
      });
    }

    res.json(result);
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

app.get('/api/orders/:id', async (req, res) => {
  try {
    const o = await db.get('SELECT * FROM orders WHERE id = ?', [req.params.id]);
    if (!o) return res.status(404).json({ error: 'Đơn hàng không tồn tại!' });

    const items = await db.all('SELECT * FROM order_items WHERE order_id = ?', [o.id]);
    const history = await db.all('SELECT * FROM order_status_history WHERE order_id = ? ORDER BY time ASC', [o.id]);

    res.json({
      id: o.id,
      userId: o.user_id,
      customer: {
        name: o.customer_name,
        phone: o.customer_phone,
        email: o.customer_email,
        address: o.customer_address,
        city: o.customer_city,
        district: o.customer_district,
        ward: o.customer_ward,
        note: o.customer_note,
        giftWrap: !!o.gift_wrap,
      },
      items: items.map(i => ({
        productId: i.product_id,
        productName: i.product_name,
        productImage: i.product_image,
        price: i.price,
        quantity: i.quantity,
      })),
      total: o.total,
      paymentMethod: o.payment_method,
      status: o.status,
      createdAt: o.created_at,
      updatedAt: o.updated_at,
      statusHistory: history.map(h => ({ status: h.status, time: h.time, note: h.note })),
    });
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

app.post('/api/orders', async (req, res) => {
  try {
    const { customerInfo, items, paymentMethod, userId } = req.body;
    if (!items || items.length === 0) {
      return res.status(400).json({ success: false, message: 'Giỏ hàng trống!' });
    }

    const orderId = 'ORD-' + Date.now();
    const now = new Date().toISOString();

    await db.transaction(async () => {
      let total = 0;
      for (const item of items) {
        const p = await db.get('SELECT * FROM products WHERE id = ?', [item.productId]);
        if (!p || p.stock < item.quantity) {
          throw new Error(`Sản phẩm "${p ? p.name : item.productId}" không đủ hàng!`);
        }
        total += p.price * item.quantity;
      }

      if (customerInfo.giftWrap) total += 15000;

      await db.run(
        `INSERT INTO orders 
         (id, user_id, customer_name, customer_phone, customer_email, customer_address, customer_city, customer_district, customer_ward, customer_note, gift_wrap, total, payment_method, status, created_at, updated_at)
         VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, 'pending', ?, ?)`,
        [
          orderId,
          userId || null,
          customerInfo.name,
          customerInfo.phone,
          customerInfo.email || '',
          customerInfo.address,
          customerInfo.city,
          customerInfo.district || '',
          customerInfo.ward || '',
          customerInfo.note || '',
          customerInfo.giftWrap ? 1 : 0,
          total,
          paymentMethod,
          now, now,
        ]
      );

      for (const item of items) {
        const p = await db.get('SELECT * FROM products WHERE id = ?', [item.productId]);
        const pImages = JSON.parse(p.images || '[]');
        
        await db.run(
          `INSERT INTO order_items (order_id, product_id, product_name, product_image, price, quantity)
           VALUES (?, ?, ?, ?, ?, ?)`,
          [orderId, p.id, p.name, pImages[0] || '', p.price, item.quantity]
        );

        await db.run(
          'UPDATE products SET stock = stock - ?, sold = sold + ? WHERE id = ?',
          [item.quantity, item.quantity, p.id]
        );
      }

      await db.run(
        'INSERT INTO order_status_history (order_id, status, time, note) VALUES (?, ?, ?, ?)',
        [orderId, 'pending', now, 'Đơn hàng được tạo']
      );
    });

    res.json({ success: true, orderId });
  } catch (err) {
    res.status(400).json({ success: false, message: err.message });
  }
});

app.patch('/api/orders/:id/status', async (req, res) => {
  try {
    const { status, note } = req.body;
    const orderId = req.params.id;
    const now = new Date().toISOString();

    const order = await db.get('SELECT * FROM orders WHERE id = ?', [orderId]);
    if (!order) return res.status(404).json({ error: 'Đơn hàng không tồn tại!' });

    await db.transaction(async () => {
      if (status === 'cancelled' && order.status !== 'cancelled') {
        const items = await db.all('SELECT * FROM order_items WHERE order_id = ?', [orderId]);
        for (const item of items) {
          await db.run(
            'UPDATE products SET stock = stock + ?, sold = MAX(0, sold - ?) WHERE id = ?',
            [item.quantity, item.quantity, item.product_id]
          );
        }
      }

      await db.run(
        'UPDATE orders SET status = ?, updated_at = ? WHERE id = ?',
        [status, now, orderId]
      );

      await db.run(
        'INSERT INTO order_status_history (order_id, status, time, note) VALUES (?, ?, ?, ?)',
        [orderId, status, now, note || '']
      );
    });

    res.json({ success: true });
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

// ── Reviews
app.get('/api/reviews', async (req, res) => {
  try {
    const { productId } = req.query;
    let sql = 'SELECT * FROM reviews';
    const params = [];

    if (productId) {
      sql += ' WHERE product_id = ?';
      params.push(productId);
    }

    sql += ' ORDER BY created_at DESC';
    const rows = await db.all(sql, params);

    const reviews = rows.map(r => ({
      id: r.id,
      productId: r.product_id,
      orderId: r.order_id,
      customerName: r.customer_name,
      rating: r.rating,
      comment: r.comment,
      createdAt: r.created_at,
    }));

    res.json(reviews);
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

app.post('/api/reviews', async (req, res) => {
  try {
    const { productId, orderId, customerName, rating, comment } = req.body;

    let targetOrderId = (orderId || '').trim();
    if (!targetOrderId || targetOrderId.toLowerCase() === 'demo' || targetOrderId.toLowerCase() === 'demo-order') {
      targetOrderId = 'demo-order';
    } else {
      const order = await db.get('SELECT * FROM orders WHERE id = ?', [targetOrderId]);
      if (!order) {
        return res.status(400).json({ success: false, message: `Mã đơn hàng "${targetOrderId}" không tồn tại. Thử dùng mã mẫu "demo-order"!` });
      }
      if (order.status !== 'completed') {
        return res.status(400).json({ success: false, message: `Đơn hàng "${targetOrderId}" chưa hoàn thành (trạng thái: ${order.status}). Chỉ đánh giá được sau khi đơn hàng chuyển sang 'Hoàn thành'!` });
      }
    }

    const existing = await db.get(
      'SELECT * FROM reviews WHERE product_id = ? AND order_id = ?',
      [productId, targetOrderId]
    );
    if (existing) {
      return res.status(400).json({ success: false, message: 'Đơn hàng này đã gửi đánh giá cho sản phẩm này rồi!' });
    }

    const reviewId = 'rv-' + Date.now();
    const now = new Date().toISOString();

    await db.transaction(async () => {
      await db.run(
        `INSERT INTO reviews (id, product_id, order_id, customer_name, rating, comment, created_at)
         VALUES (?, ?, ?, ?, ?, ?, ?)`,
        [reviewId, productId, targetOrderId, customerName, rating, comment, now]
      );

      const productReviews = await db.all('SELECT rating FROM reviews WHERE product_id = ?', [productId]);
      const avg = productReviews.reduce((sum, r) => sum + r.rating, 0) / productReviews.length;
      const roundedAvg = Math.round(avg * 10) / 10;

      await db.run(
        'UPDATE products SET rating = ?, review_count = ? WHERE id = ?',
        [roundedAvg, productReviews.length, productId]
      );
    });

    res.json({ success: true, id: reviewId });
  } catch (err) {
    res.status(400).json({ success: false, message: err.message });
  }
});

app.delete('/api/reviews/:id', async (req, res) => {
  try {
    const review = await db.get('SELECT * FROM reviews WHERE id = ?', [req.params.id]);
    if (!review) return res.status(404).json({ error: 'Đánh giá không tồn tại!' });

    await db.transaction(async () => {
      await db.run('DELETE FROM reviews WHERE id = ?', [req.params.id]);

      const productReviews = await db.all('SELECT rating FROM reviews WHERE product_id = ?', [review.product_id]);
      const count = productReviews.length;
      const avg = count > 0 ? Math.round((productReviews.reduce((sum, r) => sum + r.rating, 0) / count) * 10) / 10 : 0;

      await db.run(
        'UPDATE products SET rating = ?, review_count = ? WHERE id = ?',
        [avg, count, review.product_id]
      );
    });

    res.json({ success: true });
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

// ── Admin Stats
app.get('/api/stats', async (req, res) => {
  try {
    const totalOrders = (await db.get('SELECT COUNT(*) as c FROM orders')).c;
    const completedOrders = (await db.get("SELECT COUNT(*) as c FROM orders WHERE status = 'completed'")).c;
    const pendingOrders = (await db.get("SELECT COUNT(*) as c FROM orders WHERE status = 'pending'")).c;
    const totalRevenueRow = await db.get("SELECT SUM(total) as rev FROM orders WHERE status = 'completed'");
    const totalRevenue = totalRevenueRow.rev || 0;
    const totalProducts = (await db.get('SELECT COUNT(*) as c FROM products')).c;
    const totalReviews = (await db.get('SELECT COUNT(*) as c FROM reviews')).c;
    const totalUsers = (await db.get('SELECT COUNT(*) as c FROM users')).c;

    res.json({
      totalOrders,
      completedOrders,
      pendingOrders,
      totalRevenue,
      totalProducts,
      totalReviews,
      totalUsers,
    });
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

// ── Reset Database Endpoint
app.post('/api/reset', async (req, res) => {
  try {
    await db.exec(`
      DELETE FROM reviews;
      DELETE FROM order_status_history;
      DELETE FROM order_items;
      DELETE FROM orders;
      DELETE FROM products;
      DELETE FROM categories;
      DELETE FROM users;
      DELETE FROM admins;
    `);
    await seedDatabase();
    await db.run(
      'INSERT INTO admins (username, password, name, role) VALUES (?, ?, ?, ?)',
      ['admin', 'admin123', 'Quản trị viên', 'admin']
    );
    await db.run(
      'INSERT INTO users (email, password, name, phone, address, city) VALUES (?, ?, ?, ?, ?, ?)',
      ['khach@gmail.com', '123456', 'Nguyễn Thị Khách', '0987654321', '456 Lê Lợi, Phường 1', 'TP. Hồ Chí Minh']
    );
    res.json({ success: true, message: 'Đã reset cơ sở dữ liệu về mặc định!' });
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

// Page routes without .html extension
const pages = ['products', 'product-detail', 'cart', 'checkout', 'orders'];
pages.forEach(page => {
  app.get(`/${page}`, (req, res) => {
    res.sendFile(path.join(__dirname, `${page}.html`));
  });
});
app.get('/admin', (req, res) => res.sendFile(path.join(__dirname, 'admin/index.html')));
app.get('/admin/products', (req, res) => res.sendFile(path.join(__dirname, 'admin/products.html')));
app.get('/admin/orders', (req, res) => res.sendFile(path.join(__dirname, 'admin/orders.html')));
app.get('/admin/reviews', (req, res) => res.sendFile(path.join(__dirname, 'admin/reviews.html')));

// Fallback → index
app.get('*', (req, res) => {
  res.sendFile(path.join(__dirname, 'index.html'));
});

// Start Server
initDatabase().then(() => {
  app.listen(PORT, () => {
    console.log(`==================================================`);
    console.log(`🚀 Handmade Corner Server is running!`);
    console.log(`🌐 Website URL: http://localhost:${PORT}`);
    console.log(`🔐 Admin Login: http://localhost:${PORT}/admin/login.html`);
    console.log(`📊 REST API URL: http://localhost:${PORT}/api`);
    console.log(`💾 Database: SQLite (database.sqlite)`);
    console.log(`==================================================`);
  });
}).catch(err => {
  console.error('Failed to start server:', err);
});
