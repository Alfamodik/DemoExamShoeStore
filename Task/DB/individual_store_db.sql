DROP SCHEMA IF EXISTS individual_store;
CREATE SCHEMA individual_store;
USE individual_store;

CREATE TABLE users(
	id INT PRIMARY KEY AUTO_INCREMENT,
    role ENUM('Администратор', 'Менеджер', 'Авторизированный клиент'),
    surname VARCHAR(45),
    name VARCHAR(45),
    patronymic VARCHAR(45),
    login VARCHAR(100),
    password VARCHAR(100)
);

CREATE TABLE pick_up_points(
	id INT PRIMARY KEY AUTO_INCREMENT,
    post_index VARCHAR(6),
    sity VARCHAR(100),
    street VARCHAR(100),
    home_number VARCHAR(100)
);

CREATE TABLE suppliers(
	id INT PRIMARY KEY AUTO_INCREMENT,
    supplier VARCHAR(45)
);

CREATE TABLE product_categories(
	id INT PRIMARY KEY AUTO_INCREMENT,
    product_category VARCHAR(45)
);

CREATE TABLE manufacturers(
	id INT PRIMARY KEY AUTO_INCREMENT,
    manufacturer VARCHAR(45)
);

CREATE TABLE products(
    article VARCHAR(10) PRIMARY KEY,
    product VARCHAR(45),
    unit VARCHAR(10),
    cost DECIMAL(10,2),
    supplier_id INT,
    manufacturer_id INT,
    product_category_id INT,
    discount DECIMAL(5,2),
    amount_in_storage INT,
    description VARCHAR(255),
    image LONGBLOB,
    FOREIGN KEY (supplier_id) REFERENCES suppliers(id),
    FOREIGN KEY (manufacturer_id) REFERENCES manufacturers(id),
    FOREIGN KEY (product_category_id) REFERENCES product_categories(id)
);

CREATE TABLE orders(
	id INT PRIMARY KEY AUTO_INCREMENT,
	product_article VARCHAR(10),
    amount INT,
    date_order DATE,
    date_delivery DATE,
    pick_up_point_id INT,
    user_id INT,
    code VARCHAR(10),
    status ENUM('Завершен', 'Новый '),
    FOREIGN KEY (product_article) REFERENCES products(article),
    FOREIGN KEY (pick_up_point_id) REFERENCES pick_up_points(id),
    FOREIGN KEY (user_id) REFERENCES users(id)
);