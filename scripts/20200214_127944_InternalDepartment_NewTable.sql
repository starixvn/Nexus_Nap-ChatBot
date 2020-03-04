USE ChatBot;

/*
  Created: 20200219
  DB: ChatBot
  Task: New table for Internal Department
*/

DROP TABLE IF EXISTS InternalDepartment;
CREATE TABLE IF NOT EXISTS InternalDepartment(
      InternalDepartmentID TINYINT UNSIGNED NOT NULL,
      InternalDepartmentName VARCHAR(30),
      PRIMARY KEY (InternalDepartmentID)
    );

INSERT INTO InternalDepartment (InternalDepartmentID, InternalDepartmentName)
  VALUES (1, 'Internal CSD');

UPDATE Recipient
  SET SourceID = 1
  WHERE RecipientTypeID = 3;