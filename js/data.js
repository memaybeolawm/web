// ============================================================
// data.js – Seed data: danh mục + sản phẩm Handmade Corner
// ============================================================

const CATEGORIES = [
  { id: 'do-len',       name: 'Đồ Len',              icon: '🧶', description: 'Thú len, túi len, hoa len đan tay' },
  { id: 'trang-suc',    name: 'Trang Sức Handmade',  icon: '💍', description: 'Vòng tay, vòng cổ, khuyên tai, nhẫn' },
  { id: 'nen-thom',     name: 'Nến & Đồ Thơm',       icon: '🕯️', description: 'Nến thơm, sáp thơm, túi thơm' },
  { id: 'hoa-handmade', name: 'Hoa Handmade',         icon: '🌸', description: 'Hoa giấy, hoa len đan tay' },
  { id: 'tui-handmade', name: 'Túi Handmade',         icon: '👜', description: 'Túi tote vẽ tay, túi đan, túi len' },
  { id: 'do-trang-tri', name: 'Đồ Trang Trí',         icon: '🎨', description: 'Móc treo tường, đồ decor, tranh handmade' },
  { id: 'thu-nhoi-bong',name: 'Thú Nhồi Bông',        icon: '🧸', description: 'Gấu len, thỏ, thú bông móc tay' },
  { id: 'do-giay',      name: 'Đồ Giấy',              icon: '📖', description: 'Sổ tay, thiệp, bookmark handmade' },
  { id: 'do-gom',       name: 'Đồ Gốm',               icon: '🏺', description: 'Cốc gốm, chậu mini, đĩa decor' },
  { id: 'qua-tang',     name: 'Quà Tặng',             icon: '🎁', description: 'Hộp quà sinh nhật handmade' },
];

const PRODUCTS = [
  // ── ĐỒ LEN ──────────────────────────────────────────────
  {
    id: 'dl-001', categoryId: 'do-len', name: 'Túi Len Tote Nhỏ',
    price: 150000, stock: 8, sold: 12,
    description: 'Túi len tote nhỏ xinh đan tay, phù hợp đựng đồ nhẹ, đi chơi dã ngoại. Chất liệu len cotton mềm mịn, màu sắc tươi tắn.',
    images: [
      'images/tui-len-tote-nho.png',
    ],
    tags: ['túi', 'len', 'tote'], rating: 4.8, reviewCount: 5,
  },
  {
    id: 'dl-002', categoryId: 'do-len', name: 'Túi Len Đựng Điện Thoại',
    price: 80000, stock: 15, sold: 20,
    description: 'Túi len nhỏ vừa điện thoại, có dây đeo vai tiện lợi. Đan móc thủ công, nhiều màu sắc lựa chọn.',
    images: [
      'images/tui-len-dien-thoai.png',
    ],
    tags: ['túi', 'len', 'điện thoại'], rating: 4.6, reviewCount: 8,
  },
  {
    id: 'dl-003', categoryId: 'do-len', name: 'Thú Len Gấu Mini',
    price: 120000, stock: 10, sold: 30,
    description: 'Gấu bông len móc tay siêu cute, size mini bỏ túi. Quà tặng ý nghĩa cho bạn bè, người thân.',
    images: [
      'images/gau-len-mini.png',
    ],
    tags: ['thú', 'len', 'gấu', 'cute'], rating: 4.9, reviewCount: 15,
  },
  {
    id: 'dl-004', categoryId: 'do-len', name: 'Thú Len Thỏ Bunny',
    price: 130000, stock: 7, sold: 18,
    description: 'Thỏ len móc tay với đôi tai dài dễ thương. Chất liệu len mịn, màu trắng kem hoặc hồng pastel.',
    images: [
      'images/tho-bunny-len.png',
    ],
    tags: ['thú', 'len', 'thỏ'], rating: 4.7, reviewCount: 9,
  },

  // ── TRANG SỨC ────────────────────────────────────────────
  {
    id: 'ts-001', categoryId: 'trang-suc', name: 'Vòng Tay Đá Tự Nhiên',
    price: 95000, stock: 20, sold: 45,
    description: 'Vòng tay đá tự nhiên phối hạt gỗ, mang lại năng lượng tích cực. Thiết kế tinh tế, phù hợp nam nữ.',
    images: [
      'images/ts-vong-tay-da.png',
    ],
    tags: ['vòng tay', 'đá', 'trang sức'], rating: 4.7, reviewCount: 22,
  },
  {
    id: 'ts-002', categoryId: 'trang-suc', name: 'Vòng Cổ Hoa Khô',
    price: 120000, stock: 12, sold: 15,
    description: 'Vòng cổ đính hoa khô thật, bọc resin trong suốt, mỗi chiếc là một tác phẩm độc đáo không chiếc nào giống chiếc nào.',
    images: [
      'images/ts-vong-co-hoa-kho.png',
    ],
    tags: ['vòng cổ', 'hoa khô', 'resin'], rating: 4.9, reviewCount: 11,
  },
  {
    id: 'ts-003', categoryId: 'trang-suc', name: 'Khuyên Tai Hoa Cúc',
    price: 65000, stock: 25, sold: 38,
    description: 'Khuyên tai hoa cúc làm từ đất sét nhẹ, nhẹ nhàng thoải mái khi đeo. Màu vàng tươi hoặc trắng tinh khôi.',
    images: [
      'images/ts-khuyen-tai-hoa-cuc.png',
    ],
    tags: ['khuyên tai', 'hoa', 'đất sét'], rating: 4.6, reviewCount: 19,
  },
  {
    id: 'ts-004', categoryId: 'trang-suc', name: 'Nhẫn Dây Bện Macrame',
    price: 45000, stock: 30, sold: 55,
    description: 'Nhẫn dây bện macrame handmade, nhiều kiểu dáng đa dạng. Chỉnh được size, phù hợp mọi ngón tay.',
    images: [
      'images/ts-nhan-macrame.png',
    ],
    tags: ['nhẫn', 'macrame', 'dây'], rating: 4.5, reviewCount: 28,
  },
  {
    id: 'ts-005', categoryId: 'trang-suc', name: 'Set Vòng Tay Friendship',
    price: 75000, stock: 18, sold: 32,
    description: 'Set 2 vòng tay friendship làm từ chỉ thêu, tặng bạn thân, người yêu. Có thể đặt chữ theo yêu cầu.',
    images: [
      'images/ts-set-vong-friendship.png',
    ],
    tags: ['vòng tay', 'friendship', 'set đôi'], rating: 4.8, reviewCount: 41,
  },

  // ── NẾN & ĐỒ THƠM ────────────────────────────────────────
  {
    id: 'nt-001', categoryId: 'nen-thom', name: 'Nến Thơm Hoa Hồng',
    price: 110000, stock: 15, sold: 27,
    description: 'Nến thơm đổ tay, hương hoa hồng nhẹ nhàng lãng mạn. Thời gian cháy 20–25 tiếng, wax đậu nành tự nhiên.',
    images: [
      'images/nt-nen-hoa-hong.png',
    ],
    tags: ['nến', 'thơm', 'hoa hồng'], rating: 4.8, reviewCount: 13,
  },
  {
    id: 'nt-002', categoryId: 'nen-thom', name: 'Nến Thơm Lavender',
    price: 100000, stock: 20, sold: 35,
    description: 'Nến thơm lavender giúp thư giãn và cải thiện giấc ngủ. Hương thơm dịu nhẹ, bình yên.',
    images: [
      'images/nt-nen-lavender.png',
    ],
    tags: ['nến', 'lavender', 'thư giãn'], rating: 4.9, reviewCount: 20,
  },
  {
    id: 'nt-003', categoryId: 'nen-thom', name: 'Túi Thơm Thảo Dược',
    price: 55000, stock: 30, sold: 42,
    description: 'Túi thơm nhồi thảo dược thiên nhiên: oải hương, hoa hồng khô, lá bạch đàn. Đặt trong tủ quần áo hoặc xe hơi.',
    images: [
      'images/nt-tui-thom-thao-duoc.png',
    ],
    tags: ['túi thơm', 'thảo dược', 'tự nhiên'], rating: 4.6, reviewCount: 16,
  },
  {
    id: 'nt-004', categoryId: 'nen-thom', name: 'Sáp Thơm Khử Mùi',
    price: 80000, stock: 12, sold: 18,
    description: 'Sáp thơm tạo hình hoa, dùng để khử mùi phòng và tủ đồ. Hiệu quả kéo dài 2–3 tháng.',
    images: [
      'images/nt-sap-thom-khu-mui.png',
    ],
    tags: ['sáp thơm', 'khử mùi', 'phòng'], rating: 4.7, reviewCount: 9,
  },

  // ── HOA HANDMADE ──────────────────────────────────────────
  {
    id: 'hh-001', categoryId: 'hoa-handmade', name: 'Hoa Hồng Len – Kiểu 1',
    price: 100000, stock: 10, sold: 0,
    description: 'Hoa hồng đan bằng len cotton mềm mịn, kiểu dáng cổ điển thanh lịch. Giữ được vẻ đẹp mãi mãi, không cần tưới nước.',
    images: [
      'images/hh-hoa-hong-len-k1.png',
    ],
    tags: ['hoa', 'len', 'hoa hồng', 'mới'], rating: 0, reviewCount: 0,
  },
  {
    id: 'hh-002', categoryId: 'hoa-handmade', name: 'Hoa Cúc Len – Kiểu 2',
    price: 105000, stock: 10, sold: 0,
    description: 'Hoa cúc đan len với cánh hoa tỉ mỉ, màu vàng rực rỡ hoặc trắng tinh khôi. Trang trí bàn học, phòng ngủ.',
    images: [
      'images/hh-hoa-cuc-len-k2.png',
    ],
    tags: ['hoa', 'len', 'hoa cúc', 'mới'], rating: 0, reviewCount: 0,
  },
  {
    id: 'hh-003', categoryId: 'hoa-handmade', name: 'Hoa Tulip Len – Kiểu 3',
    price: 110000, stock: 10, sold: 0,
    description: 'Hoa tulip len mềm mại, dáng hoa thanh mảnh đẹp mắt. Có thể cắm bình hoặc tặng kèm bó hoa.',
    images: [
      'images/hh-hoa-tulip-len-k3.png',
    ],
    tags: ['hoa', 'len', 'tulip', 'mới'], rating: 0, reviewCount: 0,
  },
  {
    id: 'hh-004', categoryId: 'hoa-handmade', name: 'Hoa Hướng Dương Len – Kiểu 4',
    price: 115000, stock: 10, sold: 0,
    description: 'Hoa hướng dương len rực rỡ, tươi vui. Tượng trưng cho sự lạc quan và năng lượng tích cực.',
    images: [
      'images/hh-hoa-huong-duong-len-k4.png',
    ],
    tags: ['hoa', 'len', 'hướng dương', 'mới'], rating: 0, reviewCount: 0,
  },
  {
    id: 'hh-005', categoryId: 'hoa-handmade', name: 'Hoa Anh Đào Len – Kiểu 5',
    price: 120000, stock: 10, sold: 0,
    description: 'Hoa anh đào len tinh xảo với từng cánh hoa nhỏ xinh. Bó hoa anh đào len thích hợp làm quà Valentine, sinh nhật.',
    images: [
      'images/hh-hoa-anh-dao-len-k5.png',
    ],
    tags: ['hoa', 'len', 'anh đào', 'mới'], rating: 0, reviewCount: 0,
  },
  {
    id: 'hh-006', categoryId: 'hoa-handmade', name: 'Hoa Giấy Origami',
    price: 85000, stock: 20, sold: 14,
    description: 'Hoa giấy origami gấp tay, nhiều màu sắc sinh động. Trang trí tiệc, phòng, góc chụp ảnh.',
    images: [
      'images/hh-hoa-giay-origami.png',
    ],
    tags: ['hoa', 'giấy', 'origami'], rating: 4.5, reviewCount: 7,
  },

  // ── TÚI HANDMADE ─────────────────────────────────────────
  {
    id: 'th-001', categoryId: 'tui-handmade', name: 'Túi Tote Vẽ Tay – Mèo',
    price: 180000, stock: 10, sold: 22,
    description: 'Túi tote canvas vẽ tay hình mèo dễ thương, mực vẽ bền màu, không phai khi giặt. Size A4, đủ đựng sách vở.',
    images: [
      'images/th-tui-tote-ve-tay-meo.png',
    ],
    tags: ['túi', 'tote', 'vẽ tay', 'mèo'], rating: 4.8, reviewCount: 10,
  },
  {
    id: 'th-002', categoryId: 'tui-handmade', name: 'Túi Tote Vẽ Tay – Hoa',
    price: 175000, stock: 8, sold: 15,
    description: 'Túi tote canvas vẽ tay họa tiết hoa lá tươi sáng. Mỗi túi là một tác phẩm nghệ thuật độc đáo.',
    images: [
      'images/th-tui-tote-ve-tay-hoa.png',
    ],
    tags: ['túi', 'tote', 'vẽ tay', 'hoa'], rating: 4.7, reviewCount: 8,
  },
  {
    id: 'th-003', categoryId: 'tui-handmade', name: 'Túi Len Đan Boho',
    price: 220000, stock: 5, sold: 9,
    description: 'Túi len đan kiểu boho phóng khoáng, có tua rua trang trí. Đi biển, đi chơi cực phong cách.',
    images: [
      'images/th-tui-len-dan-boho.png',
    ],
    tags: ['túi', 'len', 'boho', 'tua rua'], rating: 4.9, reviewCount: 6,
  },
  {
    id: 'th-004', categoryId: 'tui-handmade', name: 'Túi Đan Cói Mini',
    price: 160000, stock: 12, sold: 18,
    description: 'Túi đan cói mini dáng vuông, có khóa kéo. Nhẹ nhàng, thân thiện môi trường, đi chợ hoặc đi làm.',
    images: [
      'images/th-tui-dan-coi-mini.png',
    ],
    tags: ['túi', 'cói', 'đan', 'mini'], rating: 4.6, reviewCount: 12,
  },

  // ── ĐỒ TRANG TRÍ ─────────────────────────────────────────
  {
    id: 'dtt-001', categoryId: 'do-trang-tri', name: 'Móc Treo Tường Macrame',
    price: 250000, stock: 6, sold: 11,
    description: 'Móc treo tường macrame đan tay, kiểu dáng bohemian. Trang trí phòng khách, phòng ngủ vô cùng ấm cúng.',
    images: [
      'images/dtt-moc-treo-tuong-macrame.png',
    ],
    tags: ['macrame', 'treo tường', 'decor'], rating: 4.9, reviewCount: 7,
  },
  {
    id: 'dtt-002', categoryId: 'do-trang-tri', name: 'Tranh Thêu Hoa Dại',
    price: 320000, stock: 4, sold: 6,
    description: 'Tranh thêu tay hoa dại trên vải canvas, khung gỗ thông. Tác phẩm nghệ thuật thủ công tinh tế.',
    images: [
      'images/dtt-tranh-theu-hoa-dai.png',
    ],
    tags: ['tranh', 'thêu', 'hoa', 'khung'], rating: 5.0, reviewCount: 4,
  },
  {
    id: 'dtt-003', categoryId: 'do-trang-tri', name: 'Đồ Decor Đất Sét Trang Trí',
    price: 145000, stock: 15, sold: 23,
    description: 'Set decor đất sét tạo hình thủ công: mặt trăng, ngôi sao, mây. Sơn màu pastel dịu dàng, treo tường hoặc đặt bàn.',
    images: [
      'images/dtt-do-decor-dat-set.png',
    ],
    tags: ['decor', 'đất sét', 'pastel'], rating: 4.7, reviewCount: 14,
  },
  {
    id: 'dtt-004', categoryId: 'do-trang-tri', name: 'Đèn Ngủ Len Pompom',
    price: 195000, stock: 8, sold: 10,
    description: 'Đèn ngủ trang trí tua len pompom nhiều màu sắc, ánh sáng ấm áp tạo không gian cozy.',
    images: [
      'images/dtt-den-ngu-len-pompom.png',
    ],
    tags: ['đèn', 'len', 'pompom', 'ngủ'], rating: 4.8, reviewCount: 5,
  },

  // ── THÚ NHỒI BÔNG ─────────────────────────────────────────
  {
    id: 'tnb-001', categoryId: 'thu-nhoi-bong', name: 'Gấu Teddy Len Móc Tay',
    price: 160000, stock: 8, sold: 25,
    description: 'Gấu teddy len móc tay size 20cm, bụng bông mềm mịn. Quà tặng ý nghĩa dịp sinh nhật, Valentine.',
    images: [
      'images/tnb-gau-teddy-len-moc-tay.png',
    ],
    tags: ['gấu', 'teddy', 'len', 'móc tay'], rating: 4.9, reviewCount: 18,
  },
  {
    id: 'tnb-002', categoryId: 'thu-nhoi-bong', name: 'Thỏ Bông Len Nhỏ',
    price: 140000, stock: 12, sold: 16,
    description: 'Thỏ bông len móc tay size 15cm, tai dài đáng yêu. Có thể đặt màu theo yêu cầu.',
    images: [
      'images/tnb-tho-bong-len-nho.png',
    ],
    tags: ['thỏ', 'bông', 'len', 'cute'], rating: 4.8, reviewCount: 11,
  },
  {
    id: 'tnb-003', categoryId: 'thu-nhoi-bong', name: 'Ếch Xanh Len Móc',
    price: 125000, stock: 10, sold: 20,
    description: 'Ếch xanh len móc tay siêu cute, biểu cảm hài hước. Trend trên mạng xã hội, quà tặng bạn bè hot nhất.',
    images: [
      'images/tnb-ech-xanh-len-moc.png',
    ],
    tags: ['ếch', 'len', 'cute', 'trend'], rating: 4.9, reviewCount: 30,
  },
  {
    id: 'tnb-004', categoryId: 'thu-nhoi-bong', name: 'Mèo Shorthair Len',
    price: 155000, stock: 7, sold: 14,
    description: 'Mèo shorthair len móc tay, mặt tròn trĩnh dễ thương. Có thể tùy chỉnh màu lông theo yêu cầu.',
    images: [
      'images/tnb-meo-shorthair-moc-len.png',
    ],
    tags: ['mèo', 'len', 'cute'], rating: 4.7, reviewCount: 8,
  },

  // ── ĐỒ GIẤY ───────────────────────────────────────────────
  {
    id: 'dg-001', categoryId: 'do-giay', name: 'Sổ Tay Bìa Da Vintage',
    price: 135000, stock: 15, sold: 28,
    description: 'Sổ tay bìa da giả vintage, ruột giấy kraft 100 trang. Có bookmark dây, dây cột bìa phong cách retro.',
    images: [
      'images/dg-so-tay-bia-da-vintage.png',
    ],
    tags: ['sổ tay', 'vintage', 'da', 'giấy'], rating: 4.6, reviewCount: 17,
  },
  {
    id: 'dg-002', categoryId: 'do-giay', name: 'Thiệp Sinh Nhật 3D Pop-up',
    price: 55000, stock: 25, sold: 60,
    description: 'Thiệp sinh nhật 3D pop-up handmade, mở ra là hình hoa hoặc bánh kem. Kèm phong bì và tag viết tay.',
    images: [
      'images/dg-thiep-sinh-nhat-3d-popup.png',
    ],
    tags: ['thiệp', 'sinh nhật', '3D', 'pop-up'], rating: 4.8, reviewCount: 35,
  },
  {
    id: 'dg-003', categoryId: 'do-giay', name: 'Bookmark Hoa Khô Ép',
    price: 35000, stock: 40, sold: 75,
    description: 'Bookmark ép hoa khô thật, bọc màng laminate bền đẹp. Mỗi chiếc một thiết kế khác nhau, giá trị và độc đáo.',
    images: [
      'images/dg-bookmark-hoa-kho-ep.png',
    ],
    tags: ['bookmark', 'hoa khô', 'sách'], rating: 4.7, reviewCount: 42,
  },
  {
    id: 'dg-004', categoryId: 'do-giay', name: 'Sticker Set Cute Animals',
    price: 30000, stock: 50, sold: 90,
    description: 'Bộ sticker cute handmade vẽ tay động vật, in trên giấy đề-can chất lượng cao. 20 miếng/set.',
    images: [
      'images/dg-sticker-set-cute-animals.png',
    ],
    tags: ['sticker', 'cute', 'động vật'], rating: 4.5, reviewCount: 55,
  },

  // ── ĐỒ GỐM ────────────────────────────────────────────────
  {
    id: 'gg-001', categoryId: 'do-gom', name: 'Cốc Gốm Vẽ Tay Mèo',
    price: 185000, stock: 8, sold: 12,
    description: 'Cốc gốm sứ vẽ tay hình mèo dễ thương, dung tích 300ml. Men sứ an toàn thực phẩm, có thể dùng lò vi sóng.',
    images: [
      'images/gg-coc-gom-ve-tay-meo.png',
    ],
    tags: ['cốc', 'gốm', 'mèo', 'vẽ tay'], rating: 4.8, reviewCount: 7,
  },
  {
    id: 'gg-002', categoryId: 'do-gom', name: 'Chậu Gốm Mini Cây Cảnh',
    price: 95000, stock: 15, sold: 20,
    description: 'Chậu gốm mini vẽ tay, size 8cm phù hợp cây sen đá, xương rồng. Men gốm màu pastel dịu dàng.',
    images: [
      'images/gg-chau-gom-mini-cay-canh.png',
    ],
    tags: ['chậu', 'gốm', 'cây mini', 'sen đá'], rating: 4.7, reviewCount: 13,
  },
  {
    id: 'gg-003', categoryId: 'do-gom', name: 'Đĩa Gốm Decor Tròn',
    price: 145000, stock: 10, sold: 8,
    description: 'Đĩa gốm trang trí hình tròn, họa tiết hoa lá khắc nổi. Để decor bàn ăn, kệ sách hoặc treo tường.',
    images: [
      'images/gg-dia-gom-decor-tron.png',
    ],
    tags: ['đĩa', 'gốm', 'decor', 'trang trí'], rating: 4.6, reviewCount: 5,
  },

  // ── QUÀ TẶNG ─────────────────────────────────────────────
  {
    id: 'qt-001', categoryId: 'qua-tang', name: 'Hộp Quà Sinh Nhật Handmade',
    price: 350000, stock: 5, sold: 8,
    description: 'Hộp quà sinh nhật đầy đủ: nến thơm, sổ tay, vòng tay, hoa len khô. Gói quà đẹp, có thiệp viết tay.',
    images: [
      'images/qt-hop-qua-sinh-nhat-handmade.png',
    ],
    tags: ['hộp quà', 'sinh nhật', 'set quà'], rating: 5.0, reviewCount: 6,
  },
  {
    id: 'qt-002', categoryId: 'qua-tang', name: 'Gift Box Cặp Đôi',
    price: 450000, stock: 4, sold: 5,
    description: 'Hộp quà dành cho cặp đôi: set vòng tay đôi, nến thơm, thiệp handmade. Quà Valentine, kỷ niệm ngọt ngào.',
    images: [
      'images/qt-giftbox-cap-doi.png',
    ],
    tags: ['quà đôi', 'Valentine', 'tình yêu'], rating: 4.9, reviewCount: 4,
  },
  {
    id: 'qt-003', categoryId: 'qua-tang', name: 'Giỏ Quà Tết Mini',
    price: 280000, stock: 10, sold: 0,
    description: 'Giỏ quà Tết nhỏ xinh, đựng các sản phẩm handmade: nến, sáp thơm, hoa khô, sổ tay. Trang trí đỏ vàng truyền thống.',
    images: [
      'images/qt-gio-qua-tet-mini.png',
    ],
    tags: ['giỏ quà', 'Tết', 'handmade'], rating: 0, reviewCount: 0,
  },

  // ── PHỤ KIỆN (Hoa Gửi, Gấu – đi kèm túi len) ─────────────
  {
    id: 'pk-001', categoryId: 'do-len', name: 'Phụ Kiện: Hoa Len Gắn Túi',
    price: 35000, stock: 30, sold: 40,
    description: 'Hoa len nhỏ xinh dùng gắn trang trí túi len, móc khóa hoặc balo. Nhiều màu sắc lựa chọn.',
    images: [
      'images/pk-hoa-len-gan-tui.png',
    ],
    tags: ['phụ kiện', 'hoa', 'len', 'gắn túi'], rating: 4.7, reviewCount: 20,
  },
  {
    id: 'pk-002', categoryId: 'do-len', name: 'Phụ Kiện: Gấu Len Mini',
    price: 40000, stock: 25, sold: 35,
    description: 'Gấu len mini size 5cm, gắn vào túi len hoặc làm móc khóa. Cute không cưỡng lại được!',
    images: [
      'images/pk-gau-len-gan-tui.png',
    ],
    tags: ['phụ kiện', 'gấu', 'len', 'mini'], rating: 4.8, reviewCount: 15,
  },
];

// Sample reviews
const SAMPLE_REVIEWS = [
  { id: 'rv-001', productId: 'dl-003', orderId: 'demo-order', customerName: 'Lan Anh', rating: 5, comment: 'Gấu siêu xinh, chất liệu len mềm, đóng gói cẩn thận. Mình đã mua lần 2 rồi 💕', createdAt: '2026-08-15T10:30:00Z' },
  { id: 'rv-002', productId: 'ts-002', orderId: 'demo-order', customerName: 'Minh Tú', rating: 5, comment: 'Vòng cổ quá đẹp, hoa bên trong rõ và sinh động. Chụp ảnh check-in ra hình đẹp ghê!', createdAt: '2026-08-20T14:00:00Z' },
  { id: 'rv-003', productId: 'nt-002', orderId: 'demo-order', customerName: 'Thu Hà', rating: 5, comment: 'Nến thơm lavender mình rất thích, mùi dịu nhẹ không gắt. Thắp lên là ngủ ngon ngay 😍', createdAt: '2026-09-01T09:00:00Z' },
  { id: 'rv-004', productId: 'dg-002', orderId: 'demo-order', customerName: 'Bảo Châu', rating: 4, comment: 'Thiệp 3D rất sáng tạo, bạn mình thích lắm. Giao hàng nhanh, đóng gói chắc chắn.', createdAt: '2026-09-05T11:30:00Z' },
  { id: 'rv-005', productId: 'tnb-003', orderId: 'demo-order', customerName: 'Quỳnh Như', rating: 5, comment: 'Ếch xanh trend quá trời, mua tặng cho cả nhóm bạn ai cũng mê!', createdAt: '2026-09-10T16:00:00Z' },
];

// Export
if (typeof module !== 'undefined') {
  module.exports = { CATEGORIES, PRODUCTS, SAMPLE_REVIEWS };
}
