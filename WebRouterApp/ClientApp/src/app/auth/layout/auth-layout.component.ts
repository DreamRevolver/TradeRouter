import { Component } from '@angular/core';

@Component({
  selector: 'app-auth-layout',
  template: `
    <div class="container-fluid full-width-image">
      <div class="auth-container">
        <div class="auth-box">
          <div class="container">
            <div class="card col-md-6 mt-3 mx-auto">
              <div class="card-body">
                <h4 class="card-title text-center">
                  <fa-icon icon="user-circle" size="3x"></fa-icon>
                </h4>
                <router-outlet></router-outlet>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,

  styles: [
    `
      .full-width-image {
        min-height: 100vh;
        width: 100vw;
        /* background: url('../../../assets/images/winter.jpg') no-repeat center center fixed; */
        background-size: cover;
      }

      .auth-container {
        position: absolute;
        top: 0;
        bottom: 0;
        left: 0;
        right: 0;
      }

      .auth-box {
        display: flex;
        flex-direction: column;
        justify-content: center;
        align-items: center;
        width: 100%;
        min-height: 100%;
      }

      .card {
        background-color: rgba(42, 42, 42, 0.8);
        color: #fff;
      }
    `,
  ],
})
export class AuthLayoutComponent {}
