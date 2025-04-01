# Вопросы о работе с файлами, папками и дисками в C#

1. **Какие классы в C# используется для работы с файлами, папками, дисками и чем отличаются?**
2. **Как прочитать весь текст из файла в строку?**
4. **Как открыть файл для записи и добавить новую строку, не перезаписывая его содержимое?**
5. **Как получить список всех доступных дисков на компьютере? Как узнать, существует ли указанная папка на диске?**
6. **Как получить список всех файлов и папок в указанной папке?**


# Создание приложения

## **Общие инструкции**

Приложение разделите на 3 проекта:

1. **Domain**: Сущности (`Product`) и интерфейсы (например, репозитории).  
2. **Infrastructure**: Реализация интерфейсов и подключение к PostgreSQL а также для работы с файлами.  
3. **Web API**: Контроллеры для CRUD-операций и дополнительных операций.

---

## **Часть 1: Создание API**
1. **Создание базы данных**
   - Настройте PostgreSQL и создайте таблицу `Products` с полями:  
     - `Id` (идентификатор, первичный ключ).  
     - `Name` (название продукта).  
     - `Description` (описание продукта).  
     - `Price` (цена, decimal).  
     - `StockQuantity` (количество на складе).  
     - `CategoryName` (категория продукта).  
     - `CreatedDate` (дата создания).
   - Добавьте эти данные:
     ```
     INSERT INTO Products (Name, Description, Price, StockQuantity, CategoryName, CreatedDate)
     VALUES 
     ('Laptop Pro', 'High-performance laptop with 16GB RAM and 1TB SSD.', 1200.00, 10, 'Electronics', '2024-01-01 10:00:00'),
     ('Smartphone X', 'Latest smartphone with advanced camera features.', 900.00, 25, 'Electronics', '2024-01-02 15:30:00'),
     ('Wireless Headphones', 'Noise-cancelling headphones with Bluetooth.', 150.00, 50, 'Accessories', '2024-01-03 09:45:00'),
     ('Gaming Chair', 'Ergonomic chair designed for gamers.', 300.00, 20, 'Furniture', '2024-01-04 13:00:00'),
     ('Electric Kettle', '1.7L kettle with auto shut-off.', 40.00, 100, 'Appliances', '2024-01-05 11:15:00'),
     ('Desk Lamp', 'Adjustable LED desk lamp with USB charging.', 25.00, 75, 'Furniture', '2024-01-06 18:45:00'),
     ('Coffee Maker', 'Automatic coffee maker with timer.', 80.00, 30, 'Appliances', '2024-01-07 07:30:00'),
     ('Bluetooth Speaker', 'Portable speaker with rich bass sound.', 60.00, 45, 'Accessories', '2024-01-08 14:10:00'),
     ('Smartwatch Z', 'Smartwatch with fitness tracking and notifications.', 200.00, 15, 'Electronics', '2024-01-09 12:50:00'),
     ('Electric Scooter', 'Foldable scooter with a range of 30km.', 450.00, 10, 'Transport', '2024-01-10 16:25:00'),
     ('Yoga Mat', 'Non-slip yoga mat with carrying strap.', 20.00, 120, 'Sports', '2024-01-11 08:40:00'),
     ('Dumbbell Set', 'Adjustable dumbbells for home workouts.', 100.00, 50, 'Sports', '2024-01-12 19:20:00'),
     ('Tablet Pro', 'High-performance tablet for work and entertainment.', 700.00, 12, 'Electronics', '2024-01-13 10:05:00'),
     ('Refrigerator XL', 'Large refrigerator with energy-saving features.', 1200.00, 5, 'Appliances', '2024-01-14 17:30:00'),
     ('Microwave Oven', 'Compact microwave with grill feature.', 100.00, 25, 'Appliances', '2024-01-15 13:45:00'),
     ('Office Desk', 'Spacious desk with cable management.', 150.00, 20, 'Furniture', '2024-01-16 09:10:00'),
     ('Tennis Racket', 'Professional tennis racket with cover.', 120.00, 40, 'Sports', '2024-01-17 15:55:00'),
     ('Air Purifier', 'Air purifier with HEPA filter.', 250.00, 18, 'Appliances', '2024-01-18 20:30:00'),
     ('Backpack Pro', 'Water-resistant backpack with laptop compartment.', 80.00, 60, 'Accessories', '2024-01-19 11:45:00'),
     ('E-book Reader', 'E-reader with adjustable backlight.', 150.00, 30, 'Electronics', '2024-01-20 14:15:00');

2. **Реализация API**
- Все ответы должны быть единого формата тоесть Response
   - Создайте API с CRUD-операциями для работы с таблицей `Products`.  
     - **Создание нового продукта (Create)**:
       - Эндпоинт: `POST /api/products`
       - Принимает данные нового продукта и добавляет их в базу данных.
     - **Получение продуктов (Read)**:
       - Эндпоинт: `GET /api/products`
       - Возвращает список всех продуктов.
       - Эндпоинт: `GET /api/products/{id}`
       - Возвращает данные продукта по `id`.
     - **Обновление продукта (Update)**:
       - Эндпоинт: `PUT /api/products/{id}`
       - Принимает данные обновления и обновляет информацию о продукте по `id`.
     - **Удаление продукта (Delete)**:
       - Эндпоинт: `DELETE /api/products/{id}`
       - Удаляет продукт из базы данных по `id`.

3. **Дополнительные методы API**

   1. **Экспорт данных в файл**
      - **Эндпоинт:** `/api/products/export`
      - **Описание:** Получает все данные из базы данных `Products` и сохраняет их в файл `products.txt` если нету файла тогда создаёт, где каждая строка это один объект каждые поля разделены вергулом.
      - **Формат файла:**  
        ```
        1,Example Product,Example Description,100.00,10,Electronics,2024-12-23
        2,Another Product,Another Description,50.00,5,Books,2024-12-22
        ```
      - **Действия:**
        - Подключается к базе данных.
        - Читает все записи из таблицы `Products`.
        - Сохраняет каждую запись в файл, строки разделяются запятыми.

   2. **Импорт данных из файла**
      - **Эндпоинт:** `/api/products/import`
      - **Описание:** Читает данные из файла `add.txt` и добавляет их в базу данных.
      - **Действия:**
        - Открывает указанный файл.
        - Читает строки, проверяет их корректность (например, формат цены, даты).
        - Добавляет каждую строку как новую запись в таблицу `Products`.
   
   3. **Обновление данных из файла**
      - **Эндпоинт:** `/api/products/update-from-file`
      - **Описание:** Читает данные из файла `edit.txt` и обновляет соответствующие записи в базе данных.
      - **Действия:**
        - Открывает файл.
        - Сравнивает каждую строку с существующими записями в таблице `Products` (по `Id`).
        - Если запись с указанным `Id` существует, обновляет её данные (например, `Price`, `StockQuantity`).
        - Игнорирует строки, если `Id` не найден в базе.
   4. **Анализ базы данных и вывод статистики в файл**
      - **Эндпоинт:** `/api/products/analyze`
      - **Описание:** Анализирует данные в таблице `Products` и сохраняет статистику в файл `static.txt`.
      - **Действия:**
        - Подключается к базе данных.
        - Анализирует данные:
          - Общее количество продуктов.
          - Средняя цена продуктов.
          - Общая сумма остатков на складе (`StockQuantity`).
        - Формат данных в файле:
          ```
          Total Products: 100
          Average Price: 75.50
          Total Stock Quantity: 500
          ```
        - Сохраняет результаты анализа в файл `static.txt`.
---

## **Критерии реализации**
1. **Корректная настройка базы данных PostgreSQL**.
2. **Реализация всех CRUD-операций API**.
3. **Реализация дополнительных методов API для работы с файлами**.
4. **Тестирование API**: Убедитесь, что все методы работают корректно.
