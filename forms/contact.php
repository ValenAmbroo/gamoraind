<?php
  
  if ($_SERVER["REQUEST_METHOD"] == "POST") {
      $to = "valenambrosetti74@gmail.com";
      $subject = $_POST['subject'];
      $message = "Nombre: " . $_POST['name'] . "\n";
      $message .= "Email: " . $_POST['email'] . "\n\n";
      $message .= "Mensaje:\n" . $_POST['message'];
      $headers = "From: " . $_POST['email'];
  
      if (mail($to, $subject, $message, $headers)) {
          echo "Mensaje enviado exitosamente.";
      } else {
          echo "Error al enviar el mensaje.";
      }
  }

  
?>
