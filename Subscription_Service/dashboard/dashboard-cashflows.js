/* eslint-disable n/no-path-concat */
const ADMIN = 'admin'
const jsdom = require('jsdom')
const { JSDOM } = jsdom
const FileSystem = require('fs')
const utils = require('./utils').Utils
const subscriptionRepository =
    require('../persistence/subscriptionRepository').SubscriptionRepository
const plansService = require('../service/PlanService').PlanService
const responseModule = require('../service/response')
const Response = responseModule.Response

// TODO depois alterar isto! não é para 7 meses, mas sim para n!!!
const tableHeaders = [' ', 'T1', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7']

// Construção dos cabeçalhos, para os n meses especificados no pedido
function buildTableHeaders(months, mode) {
    const headers = []
    headers.push(' ')
    if (mode == 'future') {
        for (let i = 0; i < months; i++) {
            headers.push(`T${i + 1}`)
        }
    } else if (mode == 'past') {
        for (let i = 0; i < months; i++) {
            headers.push(`T-${i + 1}`)
        }
    }

    return headers
}

async function createData(mode, months) {
    const data = {}
    const now = new Date()

    // Get plans
    const plans = await plansService.getPlans('Admin', ['admin'])
    const plansData = Object.values(plans.data)
    let monthlyRevenue
    let monthlyConstant
    const totalRevenue = []
    for (let i = 0; i < plansData.length; i++) {
        const planMonthlyRevenue = []

        if (mode == 'future') {
            const currentMonthlyActive = []
            // nº de subscrições mensais ativas vão ser contantes
            const currentActive =
                await subscriptionRepository.getActiveSubscriptions(
                    now.getMonth() + i,
                    now.getFullYear(),
                    plansData[i].id
                )

            currentActive.forEach((sub) => {
                if (sub.type == 'monthly') {
                    currentMonthlyActive.push(sub)
                }
            })

            // calcula o revenue mensal de acordo com as subscrições ativas atuais
            monthlyConstant = currentMonthlyActive[0]
                ? parseFloat(currentMonthlyActive[0].subscriptionFee)
                : 0

            for (let monthIndex = 1; monthIndex <= months; monthIndex++) {
                monthlyRevenue = monthlyConstant
                const date = new Date(
                    now.getFullYear(),
                    now.getMonth() + monthIndex,
                    1
                )
                const month = date.getMonth()
                const year = date.getFullYear()

                // Safeguard empty array
                if (!planMonthlyRevenue[monthIndex - 1]) {
                    planMonthlyRevenue[monthIndex - 1] = 0
                }
                if (!totalRevenue[monthIndex - 1]) {
                    totalRevenue[monthIndex - 1] = 0
                }
                planMonthlyRevenue[monthIndex - 1] = +(
                    planMonthlyRevenue[monthIndex - 1] + monthlyRevenue
                ).toFixed(2)
                totalRevenue[monthIndex - 1] = +(
                    totalRevenue[monthIndex - 1] + monthlyRevenue
                ).toFixed(2)

                // vai buscar as subscrições anuais cuja renew date seja inferior ao mês atual
                const annualSubsRevenue =
                    await subscriptionRepository.getMonthlyRevenueCashflow(
                        month,
                        year,
                        plansData[i].id
                    )
                planMonthlyRevenue[monthIndex - 1] = +(
                    planMonthlyRevenue[monthIndex - 1] + annualSubsRevenue
                ).toFixed(2)
                totalRevenue[monthIndex - 1] = +(
                    totalRevenue[monthIndex - 1] + annualSubsRevenue
                ).toFixed(2)
            }
            data[plansData[i].id] = planMonthlyRevenue
        } else if (mode == 'past') {
            for (let monthLopp = months; monthLopp >= 1; monthLopp--) {
                const date = new Date(
                    now.getFullYear(),
                    now.getMonth() - monthLopp,
                    1
                )
                const monthIndex = monthLopp - 1
                const month = date.getMonth()
                const year = date.getFullYear()

                const active =
                    await subscriptionRepository.getActiveSubscriptions(
                        month,
                        year,
                        plansData[i].id
                    )

                if (!planMonthlyRevenue[monthIndex]) {
                    planMonthlyRevenue[monthIndex] = 0
                }
                if (!totalRevenue[monthIndex]) {
                    totalRevenue[monthIndex] = 0
                }

                active.forEach((subscription) => {
                    if (subscription.type == 'monthly') {
                        planMonthlyRevenue[monthIndex] = +(
                            planMonthlyRevenue[monthIndex] +
                            parseFloat(subscription.subscriptionFee)
                        ).toFixed(2)

                        totalRevenue[monthIndex] = +(
                            planMonthlyRevenue[monthIndex] +
                            parseFloat(subscription.subscriptionFee / 12)
                        ).toFixed(2)
                    } else if (subscription.type == 'annual') {
                        planMonthlyRevenue[monthIndex] = +(
                            planMonthlyRevenue[monthIndex] +
                            parseFloat(subscription.subscriptionFee / 12)
                        ).toFixed(2)

                        totalRevenue[monthIndex] = +(
                            totalRevenue[monthIndex] +
                            parseFloat(subscription.subscriptionFee / 12)
                        ).toFixed(2)
                    }
                })
            }
            data[plansData[i].id] = planMonthlyRevenue
        }
    }
    data.Total = totalRevenue
    return data
}

// Preenchimento da página html
async function fillTable(mode, months) {
    const data = await createData(mode, months)
    const headers = buildTableHeaders(months, mode)

    // !! Criação do document JSDOM onde serão feitas as alterações
    const dom = await JSDOM.fromFile('dashboard/dashboard-cashflows.html')
    const document = dom.window.document

    // !! Criação da Tabela
    const cashflowDiv = document.querySelector('div.cashflowBoard')
    while (cashflowDiv.firstChild) {
        cashflowDiv.removeChild(cashflowDiv.firstChild)
    } // limpa a dashboard: remove os filhos da plansDiv (se existirem)

    // cria a tabela
    const cashflowTable = document.createElement('table')
    cashflowTable.className = 'dashboardTable'

    // cria o grupo do cabeçalho da tabela
    const cashflowTableHead = document.createElement('thead')
    cashflowTableHead.className = 'dasboardTableHead'

    // cria a linha do cabeçalho
    const cashflowTableHeaderRow = document.createElement('tr')
    cashflowTableHeaderRow.className = 'dashboardTableHeaderRow'

    // percorre o array dos headers para os colocar no cabeçalho da tabela
    for (let index = 0; index < headers.length; index++) {
        const cashflowHeader = document.createElement('th')
        cashflowHeader.innerHTML = headers[index]
        cashflowTableHeaderRow.append(cashflowHeader)
    }

    // associa a lista de cabeçalhos ao elemento de grupo do cabeçalho
    cashflowTableHead.append(cashflowTableHeaderRow)
    // associa o grupo à tabela
    cashflowTable.append(cashflowTableHead)

    // cria o corpo da tabela
    const cashflowTableBody = document.createElement('tbody')
    cashflowTableBody.className = 'dashboardTable-Body'

    for (const element in data) {
        // criação das linhas das tabelas
        const line = document.createElement('tr')
        line.className = 'dashboard-cashflows'

        // criação do conteúdo das linhas
        const contentRow1 = document.createElement('td')
        contentRow1.innerHTML = element
        line.append(contentRow1)

        for (let index = 0; index < data[element].length; index++) {
            const contentCol = document.createElement('td')
            contentCol.innerHTML = `${data[element][index]}€`
            line.append(contentCol)
        }
        cashflowTable.append(line)
    }

    cashflowDiv.append(cashflowTable) // atribui a tabela criada como corpo da div

    return Promise.resolve(dom.serialize().toString())
}

async function handleCashFlowDashboard(req, res) {
    const response = new Response(res)
    const roles = req.headers.roles
    // TODO: The role should not be admin bust product-manager, but since we do not have a user with this role yet, we use admin for the moment.
    const allowed = [ADMIN]
    if (utils.checkSomeInArray(roles, allowed)) {
        if (!req.body.callback) {
            return response.badRequest(
                'You must pass a callback url in the request body'
            )
        } else {
            const mode = req.query.mode ? req.query.mode : 'future'
            const months = req.query.months ? parseInt(req.query.months) : 7
            // Has callback url
            const callback = req.body.callback
            try {
                const url = new URL(callback)
            } catch (err) {
                return response.badRequest('You must pass a valid callback url')
            }
            // Has valid callback url
            response.ok()
            const dashboard = await fillTable(mode, months)
            try {
                await utils.postBack(callback, { dashboard })
            } catch (err) {
                console.log(err.message)
            }
        }
    } else {
        return response.notAllowed(
            'You do not have permissions to perform this action'
        )
    }
}

exports.DashboardCashflows = function () {
    return {
        handleCashFlowDashboard
    }
}
