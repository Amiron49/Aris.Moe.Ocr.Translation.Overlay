name: honyaku-api
services:
- name: web
  git:
    repo_clone_url: https://github.com/Amiron49/Aris.Moe.Ocr.Translation.Overlay.git
    branch: master
  dockerfile_path: Aris.Moe.OverlayTranslate.Server.AspNetCore/Dockerfile
