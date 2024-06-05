import { MatSnackBar } from '@angular/material/snack-bar';

export function openSnackBar(
  message: string,
  action: string,
  duration: number,
  snackBar: MatSnackBar
): void {
  snackBar.open(message, action, {
    duration: duration,
  });
}
