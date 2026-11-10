-- Orders and order lines.
--
-- An order is a permanent record of a completed checkout. The cart
-- it came from is session-based and is cleared on payment, so the
-- order must snapshot everything needed to show history later:
--   - product_name / unit_price are copies, not joins, because a
--     product can later be deleted or re-priced and the history
--     must still show what was actually paid.
--   - product_id is kept without a FK for the same reason (link
--     for a product image/details if it still exists).
-- One user has many orders; one order has many lines.
CREATE TABLE orders (
    id              INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id         INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    payment_method  VARCHAR(20) NOT NULL,
    total           NUMERIC(10, 2) NOT NULL CHECK (total >= 0),
    created_at_utc  TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE order_lines (
    id            INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    order_id      INTEGER NOT NULL REFERENCES orders(id) ON DELETE CASCADE,
    product_id    INTEGER,                     -- no FK: product may be deleted later
    product_name  VARCHAR(200) NOT NULL,
    unit_price    NUMERIC(10, 2) NOT NULL CHECK (unit_price >= 0),
    quantity      INTEGER NOT NULL CHECK (quantity > 0),
    line_total    NUMERIC(10, 2) NOT NULL CHECK (line_total >= 0)
);

-- Fast lookup of "all orders for one user" for the order history page.
CREATE INDEX IX_orders_user_id ON orders(user_id);