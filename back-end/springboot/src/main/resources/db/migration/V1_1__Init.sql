CREATE TABLE "TodoLists" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_TodoLists" PRIMARY KEY AUTOINCREMENT,
    "title" TEXT NULL
);

CREATE TABLE "Todos" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_Todos" PRIMARY KEY AUTOINCREMENT,
    "title" TEXT NULL,
    "creation_date" TIMESTAMP,
    "done" INTEGER,
    "priority" INTEGER,
    "todo_list_id" INTEGER NULL,
    CONSTRAINT "FK_Todos_TodoLists_TodoListId" FOREIGN KEY ("todo_list_id") REFERENCES "TodoLists" ("id") ON DELETE RESTRICT
);

CREATE INDEX "IX_Todos_TodoListId" ON "Todos" ("todo_list_id");
