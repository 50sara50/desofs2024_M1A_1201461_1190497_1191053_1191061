import { NgIf } from '@angular/common';
import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChange,
} from '@angular/core';

@Component({
  standalone: true,
  selector: 'password-strength',
  styleUrls: ['./password-strength.component.scss'],
  templateUrl: './password-strength.component.html',
  imports: [NgIf],
})
export class PasswordStrengthComponent implements OnChanges {
  bar1: string;
  bar2: string;
  bar3: string;

  @Input() public passwordToCheck: string;

  @Output() passwordStrength = new EventEmitter<boolean>();

  private colors = ['darkred', 'orangered', 'orange', 'yellowgreen'];

  message: string;
  messageColor: string;

  checkStrength(password: string): number {
    // Define regex patterns to check for lowercase letters, uppercase letters, numbers, and symbols
    const regex = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/;
    const lowerLetters = /[a-z]+/.test(password);
    const upperLetters = /[A-Z]+/.test(password);
    const numbers = /[0-9]+/.test(password);
    const symbols = regex.test(password);

    console.log(lowerLetters, upperLetters, numbers, symbols);

    // Calculate the strength based on various criteria
    let strength = 0;

    // Check if password meets minimum length requirement and includes a combination of lowercase letters, uppercase letters, numbers, and symbols
    if (
      password.length >= 12 &&
      lowerLetters &&
      upperLetters &&
      numbers &&
      symbols
    ) {
      strength = 3;
    } else if (
      password.length >= 12 &&
      ((lowerLetters && numbers) ||
        (upperLetters && numbers) ||
        (lowerLetters && symbols))
    ) {
      strength = 2;
    } else if (password.length >= 12) {
      strength = 1;
    }
    return strength;
  }

  ngOnChanges(changes: { [propName: string]: SimpleChange }): void {
    const password = changes['passwordToCheck'].currentValue;

    this.setBarColors(4, '#DDD');

    if (password) {
      const pwdStrength = this.checkStrength(password);
      pwdStrength === 3
        ? this.passwordStrength.emit(true)
        : this.passwordStrength.emit(false);

      const color = this.getColor(pwdStrength);
      console.log(pwdStrength, color.index, color.color);
      this.setBarColors(color.index, color.color);

      switch (pwdStrength) {
        case 0:
          this.message = 'Poor';
          break;
        case 1:
          this.message = 'Not Good';
          break;
        case 2:
          this.message = 'Average';
          break;
        case 3:
          this.message = 'Good';
          break;
      }
    } else {
      this.message = '';
    }
  }

  private getColor(strength: number) {
    this.messageColor = this.colors[strength];

    return {
      index: strength,
      color: this.colors[strength],
    };
  }

  private setBarColors(count: number, color: string) {
    for (let n = 0; n <= count; n++) {
      (this as any)['bar' + n] = color;
    }
  }
}
