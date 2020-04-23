## Criando Web API no dotnet core

### Pre Requisitos
1. Instalar [Java 8](https://www.oracle.com/java/technologies/javase-jdk8-downloads.html)

2. Instalar [Maven](https://maven.apache.org/download.cgi)

3. Instalar IDE ou Ferramenta de Desenvolvimento
- [Eclipse JEE](https://www.eclipse.org/downloads/packages/release/2020-03/r/eclipse-ide-enterprise-java-developers-includes-incubating-components)
- [Visual Studio Code](https://code.visualstudio.com/#alt-downloads)

### Criando o Projeto
4. Criar projeto usando http://start.spring.io
- Project 'Maven Project'
- Language 'Java'
- Package 'Jar'
- Java '8'
- Dependencies 'Spring Web', 'Spring Data JPA', 'Flyway Migration'

5. Abrir projeto na sua IDE. No Eclipse JEE, 'Import...' > 'Existing Maven Projects'

6. Criar arquivos de domínio:
- src\main\java\edu\unimetrocamp\todo\domain\Todo.java
```java
package edu.unimetrocamp.todo.domain;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.Table;

@Entity
@Table(name = "Todos")
public class Todo {
    public enum PriorityEnum {
        LOW,
        MEDIUM,
        HIGH
    };

    @Id
    @GeneratedValue(strategy=GenerationType.IDENTITY)
    private Long id;

    @Column(nullable = false)
    private String title;

    @Column(name="creation_date")
	private Date creationDate;

    @Column
	private boolean done;

    @Column
	private PriorityEnum priority;

    /**
     * @return the id
     */
    public Long getId() {
        return id;
    }

    /**
     * @param id the id to set
     */
    public void setId(final Long id) {
        this.id = id;
    }

    /**
     * @return the creationDate
     */
    public Date getCreationDate() {
        return creationDate;
    }

    /**
     * @param creationDate the creationDate to set
     */
    public void setCreationDate(final Date creationDate) {
        this.creationDate = creationDate;
    }

    /**
     * @return the title
     */
    public String getTitle() {
        return title;
    }

    /**
     * @param title the title to set
     */
    public void setTitle(String title) {
        this.title = title;
    }

    /**
     * @return the done
     */
    public boolean isDone() {
        return done;
    }

    /**
     * @param done the done to set
     */
    public void setDone(boolean done) {
        this.done = done;
    }

    /**
     * @return the priority
     */
    public PriorityEnum getPriority() {
        return priority;
    }

    /**
     * @param priority the priority to set
     */
    public void setPriority(PriorityEnum priority) {
        this.priority = priority;
    }
}
```

- src\main\java\edu\unimetrocamp\todo\domain\TodoList.java
```java
package edu.unimetrocamp.todo.domain;

import java.util.Set;

import javax.persistence.Entity;
import javax.persistence.FetchType;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.OneToMany;
import javax.persistence.Table;

@Entity
@Table(name = "TodoLists")
public class TodoList {
    @Id
    @GeneratedValue(strategy=GenerationType.IDENTITY)
    private long id;

    private String title;

    @OneToMany(fetch = FetchType.LAZY)  
    private Set<Todo> todos;

    /**
     * @return the id
     */
    public long getId() {
        return id;
    }

    /**
     * @param id the id to set
     */
    public void setId(long id) {
        this.id = id;
    }

    /**
     * @return the title
     */
    public String getTitle() {
        return title;
    }

    /**
     * @param title the title to set
     */
    public void setTitle(String title) {
        this.title = title;
    }

    /**
     * @return the todos
     */
    public Set<Todo> getTodos() {
        return todos;
    }

    /**
     * @param todos the todos to set
     */
    public void setTodos(Set<Todo> todos) {
        this.todos = todos;
    }
}
```

7. Alterar arquivo POM para incluir dependências:
```xml
        <dependency>
            <groupId>org.xerial</groupId>
            <artifactId>sqlite-jdbc</artifactId>
            <version>${sqlite-jdbc.version}</version>
        </dependency>
        
        <!-- https://mvnrepository.com/artifact/com.zsoltfabok/sqlite-dialect -->
		<dependency>
		    <groupId>com.zsoltfabok</groupId>
		    <artifactId>sqlite-dialect</artifactId>
		    <version>1.0</version>
		</dependency>
```

8. Criar os repositórios para acesso ao banco de dados:
- 'src\main\java\edu\unimetrocamp\todo\domain\TodoRepository.java'
```java
package edu.unimetrocamp.todo.domain;

import org.springframework.data.repository.CrudRepository;

public interface TodoRepository extends CrudRepository<Todo, Long> {

}
```
- 'src\main\java\edu\unimetrocamp\todo\domain\TodoListRepository.java'
```java
package edu.unimetrocamp.todo.domain;

import org.springframework.data.repository.CrudRepository;

public interface TodoListRepository extends CrudRepository<TodoList, Long> {

}
```

9. Alterar '/src/main/java/edu/unimetrocamp/todo/TodoApplication.java' para incluir a seguinte anotação na definição da classe:
```java
@EnableJpaRepositories("edu.unimetrocamp.todo.domain")
```

10. Incluir puglin Flywaydb:
```xml
            <plugin>        
			    <groupId>org.flywaydb</groupId>
			    <artifactId>flyway-maven-plugin</artifactId>
			    <version>4.0.3</version>			    
			</plugin>
```

11. Criar arquivo 'src/main/resources/db/migration/V1_1__Init.sql'
```sql
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
```

12. Criar Controller 'src\main\java\edu\unimetrocamp\todo\controller\TodoController.java'
```java
package edu.unimetrocamp.todo.controller;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import edu.unimetrocamp.todo.domain.Todo;
import edu.unimetrocamp.todo.domain.TodoRepository;

@RestController
@RequestMapping({"/todos"})
public class TodoController {

   private TodoRepository repository;

   TodoController(TodoRepository todoRepository) {
       this.repository = todoRepository;
   }

   @GetMapping
   public Iterable<Todo> findAll(){
      return repository.findAll();
   }
   
   @GetMapping(path = {"/{id}"})
   public ResponseEntity<Todo> findById(@PathVariable long id){
      return repository.findById(id)
              .map(record -> ResponseEntity.ok().body(record))
              .orElse(ResponseEntity.notFound().build());
   }
   
   @PostMapping
   public Todo create(@RequestBody Todo todo){
	   System.out.println(todo.getCreationDate());
      return repository.save(todo);
   }

   @PutMapping(value="/{id}")
   public ResponseEntity<Todo> update(@PathVariable("id") long id,
                                         @RequestBody Todo todo) {
      return repository.findById(id)
              .map(record -> {
                  record.setTitle(todo.getTitle());
                  record.setCreationDate(todo.getCreationDate());
                  record.setDone(todo.isDone());
                  record.setPriority(todo.getPriority());
                  Todo updated = repository.save(record);
                  return ResponseEntity.ok().body(updated);
              }).orElse(ResponseEntity.notFound().build());
   }
   
   @DeleteMapping(path ={"/{id}"})
   public ResponseEntity<?> delete(@PathVariable long id) {
      return repository.findById(id)
              .map(record -> {
                  repository.deleteById(id);
                  return ResponseEntity.ok().build();
              }).orElse(ResponseEntity.notFound().build());
   }

} 
```

13. Alterar 'src\main\resources\application.properties'
```ini
spring.datasource.url=jdbc:sqlite:todo.db
spring.datasource.driver-class-name=org.sqlite.JDBC
spring.jpa.hibernate.ddl-auto=none
spring.jpa.hibernate.naming_strategy=org.hibernate.cfg.EJB3NamingStrategy
spring.jpa.show-sql=true
spring.jpa.properties.hibernate.dialect=org.hibernate.dialect.SQLiteDialect

spring.flyway.baselineOnMigrate=true
spring.flyway.check-location=true
spring.flyway.locations=classpath:db/migration
spring.flyway.enabled=true
```

14. Executar projeto:
```bash
mvn spring-boot:run
```

15. Usar Postman para testar
```js
HTTP POST http://localhost:8080/todos 
{
	"id": 0,
	"Title": "Teste",
	"CreationDate": "2020-04-06T17:16:40",
	"Done": false,
	"Priority": 0
}

HTTP GET http://localhost:8080/todos

HTTP PUT http://localhost:8080/todos/1 
{
	"id": 1,
	"Title": "Teste Novo",
	"CreationDate": "2020-04-06T17:16:40",
	"Done": false,
	"Priority": 1
}

HTTP DELETE http://localhost:8080/todos/1
```