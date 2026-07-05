# 01-plan-inicial.md

## 1. Configuración del IDE

- **Editor**: Visual Studio Code (v1.92)
- **Extensiones instaladas**:
  - *GitHub Copilot* – asistencia de código basada en IA.
  - *Cursor* – complementa Copilot con refactorizaciones y explicaciones.
  - *ESLint* – linting para JavaScript/HTML.
  - *Prettier* – formateo automático.
  - *Live Server* – vista previa en tiempo real del HTML.
- **Integración con asistentes de código**:
  - Copilot configurado con la clave de API de GitHub, con sugerencias habilitadas para todos los archivos del proyecto.
  - Cursor habilitado para generar fragmentos de CSS y documentación a partir de comentarios.

## 2. Rules / Skills utilizadas

- **GitHub Copilot**
  - *Rule*: "Priorizar soluciones simples y con mínima complejidad".
  - *Propósito*: Mantener el código legible y fácil de revisar.
- **Cursor**
  - *Skill*: "Generate documentation from code comments".
  - *Propósito*: Automatizar la creación de los archivos `README.md` y los planes de implementación.
- **ChatGPT (via web)**
  - *Prompt*: "Describe UI design best practices for a premium look".
  - *Propósito*: Definir la estética del proyecto (gradientes, tipografía Inter, micro‑animaciones).

## 3. Plan de Implementación Inicial (IA‑asistido)

1. **Estructura del repositorio** – Generada con la ayuda de Copilot (`git init`, creación de carpetas).
2. **Creación de la carpeta `ia_docs`** – Solicitada a Copilot para generar los archivos base.
3. **Diseño de la UI** – Prompt a ChatGPT para obtener una paleta de colores y estilos premium; luego Copilot tradujo la propuesta a CSS.
4. **Implementación del layout** – Copilot escribió el HTML y CSS iniciales; Cursor revisó y añadió comentarios de accesibilidad.
5. **Documentación de IA** – Utilizando la skill de Cursor para generar este fichero y los planes numerados.
6. **Revisión y ajuste** – Copilot ofreció refactorizaciones para mejorar la separación de estilos y la modularidad del código.

---

*Este documento se mantiene actualizado con cada iteración del proyecto asistida por IA.*
