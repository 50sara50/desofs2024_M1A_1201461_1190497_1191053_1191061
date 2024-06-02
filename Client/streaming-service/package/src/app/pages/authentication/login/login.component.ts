import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { openSnackBar } from 'src/app/utils/uiActions';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
})
export class AppSideLoginComponent {
  username: string = '';
  password: string = '';
  constructor(
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  public login(email: string, password: string): void {
    this.authService.login(email, password).subscribe({
      next: (response) => {
        if (response && response.token) {
          this.router.navigate(['/']);
        }
      },
      error: (err) => {
        this.handleError(err);
      },
    });
  }
  handleError(err: HttpErrorResponse) {
    let message = '';
    if (err.status === HttpStatusCode.InternalServerError) {
      message = 'Internal server error';
    }
    if (err.status === HttpStatusCode.TooManyRequests) {
      const retryAfter = err.headers.get('Retry-After');
      message = 'Too many requests. Please wait ' + retryAfter + ' seconds';
    } else {
      message = 'Invalid email or password';
    }
    openSnackBar(message, 'Close', 2000, this.snackBar);
  }
}
