import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';
import { map } from 'rxjs';

export const authenticationGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.isAuthenticated().pipe(
    map((response) => {
      console.log(response);
      if (response.isAuthenticated) {
        return true;
      } else {
        router.navigate(['/authentication/login']);
        return false;
      }
    })
  );
};
