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