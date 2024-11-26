import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { error } from 'console';
import { catchError, Observable, of } from 'rxjs';
import { ChatClient } from './web-api-client';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [HttpClientModule, FormsModule, CommonModule],
  providers: [ChatClient],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'client-app';
  searchResponse : string = '';
  messages: { sender: string, text: string }[] = [];
  userInput: string = '';

  constructor(
    private _chatService: ChatClient
  ) {

  }
  sendMessage() {
    if (this.userInput.trim()) {
      // Push user message
      this.messages.push({ sender: 'User', text: this.userInput });

      // Clear input
      const userMessage = this.userInput;
      this.userInput = '';

      // Simulate bot response
      setTimeout(() => {
        this.getResponse(userMessage);
        
      }, 1000); // Simulating delay
    }
  }

   getResponse(message: string){
     // Replace this logic with real API integration or custom logic
     this._chatService.callGenerateApi(message).subscribe(
       res => {
         var result = res;
         console.log(res)
         this.messages.push({ sender: 'Bot', text: result });
     },
       error=>{
     console.log(error);
       }
     )
   }

}
