CREATE TABLE user_(
	id SERIAL PRIMARY KEY,
	name VARCHAR(50) NOT NULL,
	surname VARCHAR(50) NOT NULL,
	login VARCHAR(50) NOT NULL,
	hashed_password VARCHAR(100) NOT NULL,
	email VARCHAR(100) NOT NULL UNIQUE,
	phone_number CHAR(10) NOT NULL UNIQUE
);

CREATE TABLE car_(
	id SERIAL PRIMARY KEY,
	brand VARCHAR(50) NOT NULL,
	model VARCHAR(50) NOT NULL,
	city VARCHAR(50) NOT NULL,
	mileage INT NOT NULL,
	transmission VARCHAR(50) NOT NULL,
	engine VARCHAR(50) NOT NULL,
	body VARCHAR(50) NOT NULL,
	release_year INT NOT NULL,
	drive VARCHAR(50) NOT NULL,
	cost MONEY NOT NULL,
	volume INT NOT NULL,
	description TEXT,
	date_publish DATE NOT NULL,
	photos VARCHAR(1024) NOT NULL,
	id_owner SERIAL REFERENCES users(id)
);

CREATE TABLE like_(
	id SERIAL PRIMARY KEY,
	id_user SERIAL REFERENCES users(id),
	id_car SERIAL REFERENCES cars(id)
);

CREATE TABLE response_(
	id SERIAL PRIMARY KEY,
	id_user SERIAL REFERENCES users(id),
	id_car SERIAL REFERENCES cars(id),
	id_owner SERIAL REFERENCES users(id),
	message TEXT,
	is_claim BOOLEAN NOT NULL DEFAULT FALSE
);
