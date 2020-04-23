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
