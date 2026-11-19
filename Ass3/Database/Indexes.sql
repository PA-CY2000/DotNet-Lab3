CREATE INDEX IF NOT EXISTS ix_tickets_status_desk ON tickets (status, assigned_desk_id, issued_at);
CREATE INDEX IF NOT EXISTS ix_tickets_issued_at ON tickets (issued_at);
CREATE INDEX IF NOT EXISTS ix_ticket_history_ticket ON ticket_history (ticket_id, created_at);
CREATE INDEX IF NOT EXISTS ix_activity_logs_created ON activity_logs (created_at DESC);
CREATE INDEX IF NOT EXISTS ix_alerts_unread ON alerts (is_read, created_at DESC);
CREATE INDEX IF NOT EXISTS ix_users_username ON users (username);
