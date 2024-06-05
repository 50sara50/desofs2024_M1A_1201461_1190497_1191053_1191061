import { Component } from '@angular/core';
import {
  FormGroup,
  FormControl,
  Validators,
  AbstractControl,
  ValidationErrors,
  ValidatorFn,
} from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { NewUserContract } from 'src/app/model/contract/NewUserContract';
import { AuthService } from 'src/app/services/auth.service';
import { openSnackBar } from 'src/app/utils/uiActions';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
})
export class AppSideRegisterComponent {
  constructor(
    private router: Router,
    private AuthService: AuthService,
    private snackBar: MatSnackBar
  ) {}

  passwordFieldType: string = 'password';
  confirmPasswordFieldType: string = 'password';

  public tooglePasswordFieldType() {
    this.passwordFieldType =
      this.passwordFieldType === 'password' ? 'text' : 'password';
  }

  public toogleConfirmPasswordFieldType() {
    this.confirmPasswordFieldType =
      this.confirmPasswordFieldType === 'password' ? 'text' : 'password';
  }

  form = new FormGroup(
    {
      username: new FormControl('', [
        Validators.required,
        Validators.maxLength(25),
      ]),
      name: new FormControl('', [
        Validators.required,
        Validators.maxLength(50),
      ]),
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [
        Validators.required,
        Validators.minLength(6),
        Validators.maxLength(128),
        Validators.pattern(
          /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{12,}$/
        ),
      ]),
      confirmPassword: new FormControl('', [Validators.required]),
      address: new FormControl(''),
      age: new FormControl('', [
        Validators.required,
        Validators.min(10),
        Validators.max(99),
      ]),
      isArtist: new FormControl(false), // Initialize as false
    },
    {
      validators: [this.passwordMatchValidation()],
      updateOn: 'blur',
    }
  );

  passwordMatchValidation(): ValidatorFn {
    return (form: AbstractControl): ValidationErrors | null => {
      const password = form.get('password');
      const confirmPassword = form.get('confirmPassword');
      if (
        password &&
        confirmPassword &&
        password.value !== confirmPassword.value
      ) {
        confirmPassword.setErrors({ passwordMismatch: true });
        form.setErrors({ passwordMatch: true });
        return { passwordMatch: true };
      }
      return null;
    };
  }

  get f() {
    return this.form.controls;
  }

  submit() {
    const { name, username, email, password, age, address, isArtist } =
      this.form.value;
    const userAge = parseInt(age ?? '0');
    const newUser: NewUserContract = {
      username: username,
      email: email,
      password: password,
      age: userAge,
      address: address,
      name: name,
      role: isArtist ? 'Artist' : 'Subscriber',
    };
    this.AuthService.register(newUser).subscribe({
      next: (response) => {
        if (response) {
          this.router.navigate(['/authentication/login']);
        }
      },
      error: () => {
        this.handleError();
      },
    });
  }

  toggleArtist() {
    this.form.patchValue({
      isArtist: !this.form.value.isArtist,
    });
  }

  handleError() {
    openSnackBar('Error registering user', 'Close', 2000, this.snackBar);
  }
}
