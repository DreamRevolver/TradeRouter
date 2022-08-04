import { Component } from '@angular/core';

@Component({
  selector: 'app-footer',
  template: `
    <footer class="footer">
      <hr>
      <div class="container">
        <span class="text-muted">
          &copy; {{currentYear}}
          Routing your trades!
        </span>
      </div>
    </footer>
  `,
})
export class FooterComponent {
  currentYear = new Date().getUTCFullYear();
}
