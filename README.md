<p align="center">
  <h1 align="center">Desafioo.tech</h1>
  <p align="center">
    ✨ <a href="https://desafioo.tech">https://desafioo.tech</a> ✨
    <br/>
    Participe do Desafio Tech e aprimore suas habilidades técnicas, resolvendo problemas reais e destacando-se no mercado.
  </p>
</p>
<br/>
<p align="center">
  <a href="" rel="nofollow"><img src="https://img.shields.io/badge/created%20by-@ArthurSilv4-4BBAAB.svg" alt="Criado por Arthur Souza"></a>
  <a href="https://opensource.org/licenses/MIT" rel="nofollow"><img src="https://img.shields.io/github/license/ArthurSilv4/api-desafioo.tech" alt="License"></a>
  <a href="https://github.com/arthursilv4/api-desafioo.tech" rel="nofollow"><img src="https://img.shields.io/github/stars/arthursilv4/api-desafioo.tech" alt="stars"></a>
</p>

<div align="center">
  <a href="https://arthursilv4.github.io/api-desafioo.tech/">Documentation</a>
  <span>&nbsp;&nbsp;•&nbsp;&nbsp;</span>
  <a href="https://github.com/arthursilv4/api-desafioo.tech/issues/new">Issues</a>
  <span>&nbsp;&nbsp;•&nbsp;&nbsp;</span>
  <a href="">@ArthurSilv4</a>
  <br />
</div>

<br/>
<br/>

## Introdução

Este projeto é uma plataforma onde professores e profissionais da área podem criar desafios para os usuários explorarem e aplicarem seus conhecimentos na prática. O objetivo é conectar aprendizado e experiência real por meio de desafios reais.

## Apoiadores

Se você acredita no potencial deste projeto e deseja contribuir para seu crescimento, pode apoiar financeiramente através da nossa página de apoio. Sua contribuição ajudará na manutenção, desenvolvimento de novas funcionalidades e melhorias na plataforma. Qualquer valor é bem-vindo e faz a diferença para tornar essa iniciativa ainda mais acessível.

## Arquitetura do Projeto

Este projeto é baseado em uma API desenvolvida em **.NET Core / ASP.NET**, utilizando autenticação por token JWT, banco de dados **SQL Server**, cache com **Redis** e envio de e-mails via **SMTP**.



                    ┌───────────────────────────┐
                    │         Front-end         │
                    │          (React)          │
                    └────────────┬──────────────┘
                                 │
                         🔐 Auth Token (JWT)
                                 │
                    ┌────────────▼──────────────┐
                    │         API .NET          │
                    │   (.NET Core / ASP.NET)   │
                    └────────────┬──────────────┘
                                 │
            ┌────────────────────┼────────────────────┐
            │                    │                    │
    ┌───────▼───────┐    ┌───────▼────────┐   ┌───────▼───────┐
    │   Postgres    │    │     Redis      │   │     SMTP      │
    │  (Dados App)  │    │    (Cache)     │   │ (Envio Email) │
    └───────────────┘    └────────────────┘   └───────────────┘
                         

## Tecnologias

Este projeto foi desenvolvido com as seguintes tecnologias:

- ![C#](https://img.shields.io/badge/C%23-512BD4?style=for-the-badge&logo=csharp&logoColor=white)  
- ![ASP.NET](https://img.shields.io/badge/ASP.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)  
- ![Entity Framework](https://img.shields.io/badge/Entity%20Framework-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)  
- ![Postgres](https://img.shields.io/badge/Postgres-336791?style=for-the-badge&logo=postgresql&logoColor=white)  
- ![Redis](https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white)  
- ![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)  

Quer sugerir novas tecnologias ou contribuir com o projeto? Fique à vontade para abrir uma issue ou um pull request!


## Instalação

Siga os passos abaixo para clonar e rodar o projeto:

1. **Clone o repositório:**

    ```bash
    git clone https://github.com/arthursilv4/api-desafioo.tech.git
    ```

2. **Acesse o diretório do projeto:**

   ```bash
   cd api-desafioo.tech
   ```
    
   -	Abra o Visual Studio.
   -	Clique em "Open a project or solution".
   -	Navegue até o diretório do projeto clonado e selecione o arquivo de solução (.sln).

3. **Execute o Docker Compose para iniciar os contêineres:**
   ```bash
   docker-compose up -d
   ```
4. **Aplicar as Migrações do Banco de Dados**

   ```bash
   dotnet ef database update
   ```
5. **Executar o Projeto:**

    - Selecione o projeto de inicialização no Solution Explorer.
    - Pressione F5 ou clique em "Start" para executar o projeto.

Pronto! Agora você pode começar a desenvolver e explorar o projeto.

## Contato

Se você tiver dúvidas, sugestões ou feedback, por favor, abra uma **issue** neste repositório. Fique à vontade para compartilhar suas ideias ou relatar problemas diretamente no [GitHub Issues](https://github.com/arthursilv4/api-desafioo.tech/issues).

Eu ficarei feliz em ajudar e melhorar o projeto com sua colaboração!
