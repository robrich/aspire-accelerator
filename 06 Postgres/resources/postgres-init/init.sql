-- Create the weather table
CREATE TABLE IF NOT EXISTS weatherforecasts (
    id SERIAL PRIMARY KEY,
    date DATE NOT NULL,
    temperature_c INT NOT NULL,
    summary VARCHAR(100) NOT NULL
);

-- Add sample data
INSERT INTO weatherforecasts (date, temperature_c, summary) VALUES
    ('2024-01-01', -5, 'Freezing'),
    ('2026-01-02', 0, 'Bracing'),
    ('2026-01-03', 5, 'Chilly'),
    ('2026-01-04', 10, 'Cool'),
    ('2026-01-05', 15, 'Mild'),
    ('2026-01-06', 20, 'Warm'),
    ('2026-01-07', 25, 'Balmy'),
    ('2026-01-08', 30, 'Hot'),
    ('2026-01-09', 35, 'Sweltering'),
    ('2026-01-10', 40, 'Scorching');
