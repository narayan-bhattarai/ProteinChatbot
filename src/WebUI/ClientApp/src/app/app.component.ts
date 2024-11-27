import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ChatService, EssentialProteinResponseById, ProteinByName } from './web-api-client';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [HttpClientModule, FormsModule, CommonModule],
  providers: [ChatService],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'client-app';
  viewMode: 'chat' | 'table' = 'chat'; // Tracks the current view mode
  searchResponse: string = '';
  messages: { sender: string, text: string | SafeHtml , isHtml : boolean}[] = [];
  userInput: string = '';
  uniProtData: { id: string; name: string; organism: string; function: string }[] = []; // Stores UniProt data

  
  constructor(private _chatService: ChatService,
    private sanitizer: DomSanitizer) { }

  // Function to switch views
  setViewMode(mode: 'chat' | 'table') {
    this.viewMode = mode;
    
  }


  sendMessage() {
    if (this.userInput.trim()) {
      this.messages.push({ sender: 'User', text: this.userInput, isHtml: false });
      const userMessage = this.userInput;
      this.userInput = '';
      setTimeout(() => {
        this.getResponse(userMessage);
      }, 1000);
    }
  }

  getResponse(message: string) {
    if (/^\w+\sprotein$/i.test(message.trim()))
    {
      const sanitizedSearchTerm = message.replace(/\bprotein\b/i, '').trim();
     this.searchProteinByName(sanitizedSearchTerm);
    }
    else if (/^[QP]\d{5}$/.test(message.trim()))
    {
      this.userInput = '';
      this.searchByUniProtId(message);
     
    } else {
      this.userInput = '';
      // For any other input
      this.searchAnyText(message);
    }

    
  }
  searchProteinByName(message: string) {
    this._chatService.getProteinDataByProteinName(message).subscribe(
      (res) => {
        if (res && res.length > 0) {
          // Generate markdown table for the data
          const markdownTable = this.generateTableForProtein(res);
          const sanitizedHtml = this.sanitizer.bypassSecurityTrustHtml(markdownTable);
          this.messages.push({ sender: 'Bot', text: sanitizedHtml, isHtml: true });
          
          // Push the markdown table to the chat messages
        } else {
          // Push a fallback message if no results are found
          this.messages.push({ sender: 'Bot', text: 'No protein data found for your search.', isHtml:false });
        }
      },
      (error) => {
        // Push an error message for the user
        console.error('Error fetching protein data:', error);
      }
    );
  }



  searchByUniProtId(protId: string) {
    this._chatService.extractProteinDataByUniProtId(protId).subscribe(
      (res: EssentialProteinResponseById) => {

        const formattedResponse = this.formatProteinResponse(res);
        const sanitizedHtml = this.sanitizer.bypassSecurityTrustHtml(formattedResponse);
        this.messages.push({ sender: 'Bot', text: sanitizedHtml, isHtml: true });
        
      },
      (error) => {
        console.error('Error fetching chatbot response:', error);
      }
    );
  }


  searchAnyText(message: string) {
    this._chatService.getResponseOllamaGenerateAPI(message).subscribe(
      (res) => {
        this.messages.push({ sender: 'Bot', text: res, isHtml: false });
      },
      (error) => {
        console.error('Error fetching chatbot response:', error);
      }
    );
  }

  
  generateTableForProtein(data: any[]): string {
    const headers = `
      <tr>
        <th>UniProt ID</th>
        <th>Protein Name</th>
        <th>Scientific Name</th>
        <th>Taxon ID</th>
        <th>Molecular Weight</th>
      </tr>
    `;

    const rows = data
      .map(
        (entry) => `
        <tr>
          <td>${entry.uniProtID}</td>
          <td>${entry.proteinName}</td>
          <td>${entry.scientificName}</td>
          <td>${entry.taxonID}</td>
          <td>${entry.molecularWeight}</td>
        </tr>
      `
      )
      .join('');

    return `
      <table border="1" style="width: 100%; border-collapse: collapse;">
        <thead style="background-color: #f2f2f2;">${headers}</thead>
        <tbody>${rows}</tbody>
      </table>
    `;
  }



  formatProteinResponse(res: EssentialProteinResponseById): string {
    return `
    <table border="1" style="width: 100%; border-collapse: collapse;">
      <thead style="background-color: #f2f2f2;">
        <tr>
          <th style="padding: 8px;">Field</th>
          <th style="padding: 8px;">Value</th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td style="padding: 8px;">UniProt ID</td>
          <td style="padding: 8px;">${res.primaryAccession || "N/A"}</td>
        </tr>
        <tr>
          <td style="padding: 8px;">Protein Name</td>
          <td style="padding: 8px;">${res.proteinName || "N/A"}</td>
        </tr>
        <tr>
          <td style="padding: 8px;">Scientific Name</td>
          <td style="padding: 8px;">${res.scientificName || "N/A"}</td>
        </tr>
        <tr>
          <td style="padding: 8px;">Common Name</td>
          <td style="padding: 8px;">${res.commonName || "N/A"}</td>
        </tr>
        <tr>
          <td style="padding: 8px;">Taxon ID</td>
          <td style="padding: 8px;">${res.taxonID || "N/A"}</td>
        </tr>
        <tr>
          <td style="padding: 8px;">Functions</td>
          <td style="padding: 8px; text-align: justify;">${res.functions || "N/A"}</td>
        </tr>
      </tbody>
    </table>
  `;
  }


}

