import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';

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
    if (err.status === HttpStatusCode.InternalServerError) {
      this.openSnackBar(
        'A server problem identified. Please try again later',
        'Close',
        2000
      );
    }
    if (err.status === HttpStatusCode.TooManyRequests) {
      const retryAfter = err.headers.get('Retry-After');
      this.openSnackBar(
        'Too many requests. Please wait ' + retryAfter + ' seconds',
        'Close',
        2000
      );
    } else {
      this.openSnackBar('Invalid username or password', 'Close', 2000);
    }
  }

  //TODO: This should be moved to a common service
  openSnackBar(message: string, action: string, duration: number): void {
    this.snackBar.open(message, action, {
      duration: duration,
    });
  }
}
