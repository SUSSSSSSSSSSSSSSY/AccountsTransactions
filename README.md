# AccountsTransactions

Вот такие запросы в SQL Server Management Studio:

CREATE DATABASE AccountsDB;

CREATE TABLE [Accounts] (
    [AccountID] INT PRIMARY KEY IDENTITY(10000, 1),
    [AccountNumber] NVARCHAR(20) UNIQUE,
    [Balance] DECIMAL(10, 2)
);

INSERT INTO [Accounts] ([AccountNumber], [Balance])
VALUES 
('1234567890', 5000.00),
('0987654321', 3000.00);

В результате выдаёт:

Source account money: 5000,00

Target account money: 3000,00



Перевод выполнен успешно.



Source account money: 4000,00

Target account money: 4000,00
