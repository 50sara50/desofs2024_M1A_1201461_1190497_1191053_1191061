// eslint-disable-next-line no-undef
module.exports = {
    plugins: ['@typescript-eslint'],
    extends: ["eslint:recommended", "plugin:@typescript-eslint/recommended"],
    rules: {
        "@typescript-eslint/no-namespace": "off",
        "@typescript-eslint/no-undef": "off",	
        "no-inner-declarations": "off",
        "eqeqeq": "warn" // Warns when you use == instead of ===

    },
  };