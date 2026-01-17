-- Create the database if it does not exist
IF DB_ID('weatherdb') IS NULL
BEGIN
    CREATE DATABASE [weatherdb];
END
GO

-- Use the database
USE [weatherdb];
GO

-- Create the table if it does not exist
IF OBJECT_ID('dbo.WeatherForecasts','U') IS NULL
BEGIN
    CREATE TABLE dbo.WeatherForecasts (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Date DATE NOT NULL,
        TemperatureC INT NOT NULL,
        Summary NVARCHAR(100) NULL
    );
END
GO

-- Seed sample data only if table is empty
IF NOT EXISTS (SELECT 1 FROM dbo.WeatherForecasts)
BEGIN
    INSERT INTO dbo.WeatherForecasts (Date, TemperatureC, Summary) VALUES
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
END
GO
