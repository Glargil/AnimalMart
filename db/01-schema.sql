CREATE TABLE products (
    id          INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    price       NUMERIC(10, 2) NOT NULL CHECK (price >= 0),
    name        VARCHAR(200) NOT NULL,
    description TEXT NOT NULL
);

CREATE TABLE animals (
    id        INTEGER PRIMARY KEY REFERENCES products(id) ON DELETE CASCADE,
    sex       VARCHAR(20) NOT NULL,
    species   VARCHAR(100) NOT NULL,
    birthday  DATE NOT NULL
);

CREATE TABLE items (
    id    INTEGER PRIMARY KEY REFERENCES products(id) ON DELETE CASCADE,
    stock INTEGER NOT NULL DEFAULT 0 CHECK (stock >= 0)
);

CREATE TABLE users (
    id            INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name          VARCHAR(200) NOT NULL,
    email         VARCHAR(255) NOT NULL UNIQUE,
    phone_number  VARCHAR(30),
    password_hash TEXT NOT NULL
);

-- Shopping carts
--
-- One row per user, linked via user_id. This is a one-to-one
-- relationship: every user has exactly one cart, and the FK
-- lives here (not on `users`) because a cart can only be
-- created after its user already exists — inserting the user
-- first, then the cart that references it, avoids any
-- temporary NULLs or deferred constraints.
-- ON DELETE CASCADE means deleting a user deletes their cart
-- automatically, since a cart has no meaning without its owner.
CREATE TABLE shopping_carts (
    id      INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id INTEGER NOT NULL UNIQUE REFERENCES users(id) ON DELETE CASCADE
);

-- Cart items
--
-- A cart holds many products, and a product (e.g. a specific
-- terrarium lamp) can sit in many different users' carts at
-- once — that's a many-to-many relationship, which needs its
-- own join table rather than a FK on either side alone.
-- Each row here is one product placed in one cart. cart_items
-- has its own id (rather than a composite PK on cart_id +
-- product_id) so the same product can appear more than once in
-- a single cart, matching the C# List<Product> semantics rather
-- than a de-duplicated set.
CREATE TABLE cart_items (
    id         INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    cart_id    INTEGER NOT NULL REFERENCES shopping_carts(id) ON DELETE CASCADE,
    product_id INTEGER NOT NULL REFERENCES products(id) ON DELETE CASCADE
);