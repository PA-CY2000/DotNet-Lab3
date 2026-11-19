INSERT INTO roles (role_name) VALUES ('RECEPTIONIST'), ('OFFICER'), ('SUPERVISOR'), ('ADMINISTRATOR') ON CONFLICT (role_name) DO NOTHING;
INSERT INTO services (service_name, description, average_service_minutes) VALUES
 ('Taxpayer Registration', 'Register a taxpayer with RRA.', 15),
 ('Tax Declaration', 'Assistance with tax declarations.', 20),
 ('Tax Payment', 'Process a tax payment.', 10),
 ('TIN Assistance', 'TIN registration and assistance.', 12),
 ('Tax Clearance', 'Request tax clearance documentation.', 15)
ON CONFLICT (service_name) DO NOTHING;
INSERT INTO desks (desk_name, status) VALUES ('Desk 1','OPEN'), ('Desk 2','OPEN'), ('Desk 3','OPEN'), ('Desk 4','OPEN') ON CONFLICT (desk_name) DO NOTHING;
INSERT INTO system_settings (setting_key, setting_value) VALUES
 ('LONG_WAIT_THRESHOLD','30'), ('LOW_RATING_THRESHOLD','2'), ('TICKET_PREFIX','TAX'), ('DISPLAY_REFRESH_SECONDS','5')
ON CONFLICT (setting_key) DO NOTHING;
INSERT INTO desk_services (desk_id, service_id)
SELECT d.desk_id, s.service_id FROM desks d CROSS JOIN services s
ON CONFLICT DO NOTHING;
-- The application initializer replaces this marker with a PBKDF2 hash for admin/admin123.
INSERT INTO users (full_name, username, password_hash, role_id)
SELECT 'System Administrator', 'admin', 'INITIAL_ADMIN_PASSWORD', role_id
FROM roles WHERE role_name = 'ADMINISTRATOR'
ON CONFLICT (username) DO NOTHING;
