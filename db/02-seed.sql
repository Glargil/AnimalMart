-- Items
INSERT INTO products (price, name, description) VALUES
    (349.00, 'Terrarium Heat Lamp',            '75W ceramic heat emitter for reptile enclosures.'),
    (129.00, 'Digital Hygrometer',             'Temperature and humidity gauge for terrariums.'),
    (89.00,  'Calcium Powder with D3',         'Reptile supplement, 100g, for dusting live feed.'),
    (249.00, 'Cork Bark Hide',                 'Natural cork bark hide, medium size.'),
    (159.00, 'Live Cricket Colony Starter Kit','Breeding kit with 50 crickets and habitat.');

INSERT INTO items (id, stock) SELECT id, 25 FROM products WHERE name = 'Terrarium Heat Lamp';
INSERT INTO items (id, stock) SELECT id, 40 FROM products WHERE name = 'Digital Hygrometer';
INSERT INTO items (id, stock) SELECT id, 60 FROM products WHERE name = 'Calcium Powder with D3';
INSERT INTO items (id, stock) SELECT id, 18 FROM products WHERE name = 'Cork Bark Hide';
INSERT INTO items (id, stock) SELECT id, 12 FROM products WHERE name = 'Live Cricket Colony Starter Kit';

-- Animals
INSERT INTO products (price, name, description) VALUES
    (899.00,  'Rango',  'Calm and colorful crested gecko, well-handled.'),
    (1450.00, 'Zephyr', 'Vibrant green tree python, feeding well on schedule.'),
    (299.00,  'Atlas',  'Docile Madagascar hissing cockroach, great starter insect.'),
    (650.00,  'Ember',  'Striking orange baboon tarantula, established feeder.');

INSERT INTO animals (id, sex, species, birthday)
SELECT id, 'Male', 'Correlophus ciliatus', '2025-08-10' FROM products WHERE name = 'Rango';

INSERT INTO animals (id, sex, species, birthday)
SELECT id, 'Female', 'Morelia viridis', '2025-04-22' FROM products WHERE name = 'Zephyr';

INSERT INTO animals (id, sex, species, birthday)
SELECT id, 'Male', 'Gromphadorhina portentosa', '2025-10-01' FROM products WHERE name = 'Atlas';

INSERT INTO animals (id, sex, species, birthday)
SELECT id, 'Female', 'Pterinochilus murinus', '2025-02-14' FROM products WHERE name = 'Ember';

-- Users
INSERT INTO users (name, email, phone_number, password_hash) VALUES
    ('Freja Nielsen', 'freja.nielsen@example.com', '+45 20 12 34 56', 'REPLACE_WITH_REAL_HASH_1'),
    ('Mikkel Sørensen', 'mikkel.sorensen@example.com', NULL, 'REPLACE_WITH_REAL_HASH_2');
    -- ============================================================
-- Users
-- ============================================================
INSERT INTO users (name, email, phone_number, password_hash) VALUES
    ('Freja Nielsen',   'freja.nielsen@example.com',   '+45 20 12 34 56', 'REPLACE_WITH_REAL_HASH_1'),
    ('Mikkel Sørensen', 'mikkel.sorensen@example.com', NULL,              'REPLACE_WITH_REAL_HASH_2');

INSERT INTO shopping_carts (user_id)
SELECT id FROM users WHERE email = 'freja.nielsen@example.com';

INSERT INTO shopping_carts (user_id)
SELECT id FROM users WHERE email = 'mikkel.sorensen@example.com';

-- Freja's cart: a heat lamp and a gecko
INSERT INTO cart_items (cart_id, product_id)
SELECT sc.id, p.id
FROM shopping_carts sc
JOIN users u ON u.id = sc.user_id
JOIN products p ON p.name = 'Terrarium Heat Lamp'
WHERE u.email = 'freja.nielsen@example.com';

INSERT INTO cart_items (cart_id, product_id)
SELECT sc.id, p.id
FROM shopping_carts sc
JOIN users u ON u.id = sc.user_id
JOIN products p ON p.name = 'Rango'
WHERE u.email = 'freja.nielsen@example.com';

-- Mikkel's cart: calcium powder and a tarantula
INSERT INTO cart_items (cart_id, product_id)
SELECT sc.id, p.id
FROM shopping_carts sc
JOIN users u ON u.id = sc.user_id
JOIN products p ON p.name = 'Calcium Powder with D3'
WHERE u.email = 'mikkel.sorensen@example.com';

INSERT INTO cart_items (cart_id, product_id)
SELECT sc.id, p.id
FROM shopping_carts sc
JOIN users u ON u.id = sc.user_id
JOIN products p ON p.name = 'Ember'
WHERE u.email = 'mikkel.sorensen@example.com';