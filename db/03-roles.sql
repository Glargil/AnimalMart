-- User roles. Existing users get 'Customer' automatically via the default.
ALTER TABLE users
    ADD COLUMN role VARCHAR(20) NOT NULL DEFAULT 'Customer';

-- The database only ever accepts these three values.
-- DROP first keeps the migration re-runnable even if a constraint already exists.
ALTER TABLE users DROP CONSTRAINT IF EXISTS CHK_users_role;
ALTER TABLE users
    ADD CONSTRAINT CHK_users_role CHECK (role IN ('Customer', 'Admin', 'SuperAdmin'));

-- Bootstrap exactly ONE SuperAdmin: the account that can then make Admins
-- through the app. Create it through the CreateUser page first so it gets a
-- real Argon2 hash, then promote it. Replace the email with yours:
UPDATE users SET role = 'SuperAdmin' WHERE email = 'super.admin@mail.com';