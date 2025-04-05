-- Создание таблиц
CREATE TABLE IF NOT EXISTS "Employees" (
    "EmployeeId" SERIAL PRIMARY KEY,
    "Username" VARCHAR(100) NOT NULL UNIQUE,
    "PasswordHash" VARCHAR(255) NOT NULL,
    "FullName" VARCHAR(255) NOT NULL,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS "Games" (
    "GameId" SERIAL PRIMARY KEY,
    "GameName" VARCHAR(100) NOT NULL,
    "Description" TEXT,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "GameType" VARCHAR(50) NOT NULL
);

CREATE TABLE IF NOT EXISTS "Exhibits" (
    "ExhibitId" SERIAL PRIMARY KEY,
    "EmployeeId" INTEGER NOT NULL REFERENCES "Employees"("EmployeeId"),
    "Title" VARCHAR(255) NOT NULL,
    "Description" TEXT,
    "AddedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP NULL
);

CREATE TABLE IF NOT EXISTS "ExhibitImages" (
    "ImageId" SERIAL PRIMARY KEY,
    "ExhibitId" INTEGER NOT NULL REFERENCES "Exhibits"("ExhibitId") ON DELETE CASCADE,
    "ImagePath" VARCHAR(255) NOT NULL,
    "AltText" VARCHAR(255),
    "DisplayOrder" INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS "Scores" (
    "ScoreId" SERIAL PRIMARY KEY,
    "GameId" INTEGER NOT NULL REFERENCES "Games"("GameId"),
    "PlayerName" VARCHAR(100) NOT NULL,
    "ScoreValue" INTEGER NOT NULL,
    "PlayedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Добавление тестовых данных
-- Сотрудники (пароль 'password123' с использованием PBKDF2)
INSERT INTO "Employees" ("Username", "PasswordHash", "FullName", "IsActive") VALUES
('admin', '100000.4FmEXsU/wk3xmJV5bWZe7g==.YpPsyYt4+vJJCZPNG5wLAJn7HUl8GQojoGnxIXTdPJQ=', 'Administrator', TRUE),
('guide', '100000.t+y2ZEDtQlm8F5oTxLC5Eg==.xoRIZSpmYVXXK6YgOyGbJcBquJYhbYLktPnzT9WrxPA=', 'Museum Guide', TRUE);

-- Игры
INSERT INTO "Games" ("GameName", "Description", "GameType", "IsActive") VALUES
('Art Quiz', 'Тест знаний по искусству', 'quiz', TRUE),
('Treasure Hunt', 'Игра по поиску сокровищ в музее', 'game', TRUE),
('History Timeline', 'Расставьте исторические события в правильном порядке', 'quiz', TRUE);

-- Экспонаты
INSERT INTO "Exhibits" ("EmployeeId", "Title", "Description", "AddedAt") VALUES
(1, 'Древняя амфора', 'Древнегреческая амфора, датированная V веком до н.э.', '2024-01-10 10:00:00'),
(2, 'Портрет неизвестной', 'Живопись XVIII века неизвестного художника', '2024-01-15 14:30:00');

-- Изображения экспонатов
INSERT INTO "ExhibitImages" ("ExhibitId", "ImagePath", "AltText", "DisplayOrder") VALUES
(1, '/images/amphora_main.jpg', 'Основное изображение амфоры', 1),
(1, '/images/amphora_detail.jpg', 'Детальное изображение орнамента', 2),
(2, '/images/portrait_main.jpg', 'Портрет неизвестной дамы', 1);

-- Результаты игр
INSERT INTO "Scores" ("GameId", "PlayerName", "ScoreValue", "PlayedAt") VALUES
(1, 'Иван', 85, '2024-02-01 15:20:00'),
(1, 'Мария', 92, '2024-02-02 11:45:00'),
(2, 'Алексей', 75, '2024-02-03 14:10:00'),
(3, 'Елена', 88, '2024-02-04 16:30:00');