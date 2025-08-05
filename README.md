# Teste prático dti digital – José Carlos Ribeiro Minelli - DroneDelivery
![DroneDelivery](https://img.shields.io/badge/Status-Em%20Desenvolvimento-yellow)

## Visão Geral

Projeto desenvolvido para simular um sistema de entregas por drones em áreas urbanas, considerando capacidade, alcance, prioridade de entrega e restrições (zonas proibidas). O sistema gerencia o roteamento, estados do drone, consumo de bateria e múltiplas viagens para garantir a entrega de todos os pacotes possíveis.

## Link do Miro

Organização projeto disponíveis no [quadro do Trello](https://trello.com/invite/b/6890b88aafbc4ad0815ca844/ATTI785f76d14ed51d40e2f5318e655d4adcD98C8A8A/dronedelivery-desafio-dti)

---

## Funcionalidades

* Simulação de entrega de pacotes por múltiplos drones
* Controle de capacidade máxima (peso) e alcance (distância)
* Consumo de bateria baseado na distância percorrida (ida e volta)
* Respeito a zonas proibidas para entrega
* Prioridade de entrega (Alta, Média, Baixa)
* Estados do drone modelados: Idle → Carregando → EmVoo → Entregando → Retornando → Idle
* Recarregamento automático quando bateria insuficiente para próxima entrega
* Multi-viagens para entrega completa dos pacotes
* Relatório final detalhado de entregas, pacotes não entregues, eficiência dos drones

---

## Uso de IA no projeto

* **Geração e revisão de código:** auxílio para a criação e refatoração do simulador de entregas
* **Criação de testes unitários:** elaboração de cenários para validação das regras de negócio, como capacidade, bateria e zonas proibidas
* **Boas práticas para testes unitários:** estruturação clara, testes isolados, cobertura de cenários importantes

---

## Testes Unitários Criados

* Pacote com peso zero não é entregue
* Pacote com destino em zona proibida não é entregue
* Drone recarrega automaticamente para completar entregas múltiplas
* Pacotes que somam peso maior que capacidade são entregues em múltiplas viagens
* Pacotes com distância maior que o alcance máximo do drone não são entregues e ficam no relatório de não entregues

---

## Como Executar

### Requisitos

* [.NET SDK 6.0+](https://dotnet.microsoft.com/download)
* IDE (Visual Studio, VS Code) ou linha de comando

### Passos

1. Clone o repositório:

   ```bash
   git clone https://github.com/joseminelli/Teste-DTI-JoseMinelli.git
   cd Teste-DTI-JoseMinelli
   ```

2. Compile o projeto:

   ```bash
   dotnet build
   ```

3. Execute a aplicação:

   ```bash
   dotnet run --project Teste-DTI-JoseMinelli
   ```

4. Para executar os testes unitários:

   ```bash
   dotnet test
   ```



Quer que eu gere para você um modelo de email para enviar o link do GitHub à DTI também?
