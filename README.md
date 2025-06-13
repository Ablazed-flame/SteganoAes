# 🕵️‍♂️ Text Hiding in Image using Steganography with AES Encryption

This project securely hides a secret text message inside an image using a two-layer protection system:
- **AES-256 encryption** to encrypt the message
- **LSB steganography** (using SkiaSharp) to conceal the encrypted text within image pixels

> 🔐 Even if someone finds the image, they can’t read the message without both passwords.

---

## 🧠 Features

- 🔐 AES-256 (CBC) encryption using PBKDF2 with SHA-256
- 🖼️ LSB steganography via SkiaSharp on PNG/BMP images
- 🔑 Dual password system:
  - One for AES encryption
  - One for stegano bit masking (XOR)
- 📦 Console-based user flow
- ⚠️ Robust error handling (missing files, wrong passwords, invalid format)

---

## 🏗️ How It Works

1. User provides:
   - Secret message
   - AES encryption password
   - Stegano password
   - Cover image (PNG/BMP)

2. The message is:
   - Encrypted using AES-256
   - Encoded as Base64: `Base64(salt):Base64(iv):Base64(cipher)`
   - XOR-masked with stegano key
   - Embedded into LSBs of RGB pixels using SkiaSharp

3. Extraction reverses the process:
   - Bits are read from image
   - XOR unmasked with stegano key
   - Header and length verified
   - Decrypted with AES password

---

## 🔧 Technologies Used

- **Language:** C#
- **Framework:** .NET 6.0
- **Image Library:** [SkiaSharp](https://github.com/mono/SkiaSharp)
- **Cryptography:** `System.Security.Cryptography`
- **IDE:** Visual Studio / Rider

---

## 📸 Sample Usage

