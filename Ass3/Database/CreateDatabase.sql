-- Run this while connected to the postgres maintenance database.
-- PostgreSQL does not allow CREATE DATABASE inside a transaction.
SELECT 'CREATE DATABASE taxpayer_management_db'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'taxpayer_management_db')\gexec
