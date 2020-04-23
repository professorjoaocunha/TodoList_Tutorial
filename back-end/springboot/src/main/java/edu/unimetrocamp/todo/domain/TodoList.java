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
