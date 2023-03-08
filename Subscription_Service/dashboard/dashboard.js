/* eslint-disable no-undef */
/* eslint-disable camelcase */
/* eslint-disable no-useless-escape */
/* eslint-disable n/no-path-concat */
/* global console, $, document */
/* eslint-disable prefer-const */

const ADMIN = 'admin'
const jsdom = require('jsdom')
const { JSDOM } = jsdom
const FileSystem = require('fs')
const subscriptionService =
    require('../application/SubscriptionController').SubscriptionService()
const subscriptionRepository =
    require('../persistence/subscriptionRepository').SubscriptionRepository
const plansService = require('../service/PlanService').PlanService
const responseModule = require('../service/response')
const Response = responseModule.Response
const utils = require('./utils').Utils

let tableHeaders = [
    'Plan',
    'Metric',
    't -6',
    't -5',
    't -4',
    't -3',
    't -2',
    't -1'
]

async function createData() {
    let data = {}
    const now = new Date()
    // Get all plans
    let plans = await plansService.getPlans('Admin', ['admin'])
    let plansData = Object.values(plans.data)

    const totalPlanMetrics = {
        active_subscriptions: [],
        revenue: [],
        new_subscription: [],
        canceled_subscriptions: [],
        churn_rate: []
    }

    for (let index = 0; index < plansData.length; index++) {
        const planMetrics = {
            active_subscriptions: [],
            revenue: [],
            new_subscription: [],
            canceled_subscriptions: [],
            churn_rate: []
        }
        for (let monthIndex = 6; monthIndex > 0; monthIndex--) {
            const date = new Date(
                now.getFullYear(),
                now.getMonth() - monthIndex,
                1
            )
            const month = date.getMonth()
            const year = date.getFullYear()

            // ! NEW SUBSCRIPTIONS
            const newSubs = (
                await utils.getNewSubs(month, year, plansData[index].id)
            ).length
            planMetrics.new_subscription.push(newSubs)

            // ! CANCELED SUBSCRIPTIONS
            const canceled = (
                await utils.getCanceledSubs(month, year, plansData[index].id)
            ).length
            planMetrics.canceled_subscriptions.push(canceled)

            // ! ACTIVE SUBSCRIPTIONS
            const active = (
                await utils.getActiveSubs(month, year, plansData[index].id)
            ).length
            planMetrics.active_subscriptions.push(active)

            // ! MONTHLY REVENUE
            const monthlyRevenue = await utils.getMonthlyRevenue(
                month,
                year,
                plansData[index].id
            )
            planMetrics.revenue.push(monthlyRevenue)

            // ! CHURN
            planMetrics.churn_rate.push(
                utils.calculateChurn(canceled, active, newSubs)
            )
        }
        data[index] = {
            plan: plansData[index].id,
            promoted: plansData[index].promoted,
            metrics: planMetrics
        }
        Object.keys(planMetrics).forEach((metric) => {
            const values = planMetrics[metric]
            for (let i = 0; i < values.length; i++) {
                if (!totalPlanMetrics[metric][i]) {
                    totalPlanMetrics[metric][i] = 0
                }
                totalPlanMetrics[metric][i] += values[i]
            }
        })
    }

    data[Object.keys(data).length] = {
        plan: 'TOTAL',
        promoted: false,
        metrics: totalPlanMetrics
    }
    return data
}

// TODO: preencher a grelha com informações de planos, cálculos, etc.
async function createDashboardTable() {
    const data = await createData()
    // !! Criação do document JSDOM onde serão feitas as alterações
    const dom = await JSDOM.fromFile('dashboard/dashboard.html')
    const document = dom.window.document

    // !! Criação da Tabela
    const plansDiv = document.querySelector('div.planBoard')
    while (plansDiv.firstChild) plansDiv.removeChild(plansDiv.firstChild) // limpa a dashboard: remove os filhos da plansDiv (se existirem)

    // cria a tabela
    let planTable = document.createElement('table')
    planTable.className = 'dashboardTable'

    // cria o grupo do cabeçalho da tabela
    let planTableHead = document.createElement('thead')
    planTableHead.className = 'dasboardTableHead'

    // cria a linha do cabeçalho
    let planTableHeaderRow = document.createElement('tr')
    planTableHeaderRow.className = 'dashboardTableHeaderRow'

    // percorre o array dos headers para os colocar no cabeçalho da tabela
    for (let index = 0; index < tableHeaders.length; index++) {
        let planHeader = document.createElement('th')
        planHeader.innerHTML = tableHeaders[index]
        planTableHeaderRow.append(planHeader)
    }

    // associa a lista de cabeçalhos ao elemento de grupo do cabeçalho
    planTableHead.append(planTableHeaderRow)
    // associa o grupo à tabela
    planTable.append(planTableHead)

    // cria o corpo da tabela
    let plansTableBody = document.createElement('tbody')
    plansTableBody.className = 'dashboardTable-Body'

    for (const plan in data) {
        // criação das linhas da tabela
        let line = document.createElement('tr')
        if (data[plan].promoted) {
            line.className = 'dashboard-plans-promoted'
        } else {
            line.className = 'dashboard-plans'
        }
        // criação do conteúdo das linhas
        let contentRow1 = document.createElement('td')
        contentRow1.innerHTML = data[plan].plan

        let contentRow2 = document.createElement('td')
        contentRow2.className = 'metrics-column'
        // ciclo para criar as linhas das métricas
        const metrics = data[plan].metrics

        Object.keys(metrics).forEach((metric) => {
            let metricLine = document.createElement('p')
            metricLine.className = 'metrics-line'
            metricLine.innerHTML = metric

            contentRow2.append(metricLine)
        })

        line.append(contentRow1)
        line.append(contentRow2)

        for (let index = 0; index < 6; index++) {
            let contentCol = document.createElement('td')
            Object.keys(metrics).forEach((metric) => {
                let row = document.createElement('p')
                if (metric == 'churn_rate') {
                    if (metrics[metric][index] < 0) {
                        row.className = 'values-line-green'
                    } else if (metrics[metric][index] == 0) {
                        row.className = 'values-line-yellow'
                    } else {
                        row.className = 'values-line-red'
                    }
                    row.innerHTML = `${metrics[metric][index]}%`
                } else if (metric == 'revenue') {
                    if (metrics[metric][index] == 0) {
                        row.innerHTML = '- €'
                    } else {
                        row.innerHTML = `${metrics[metric][index]}€`
                    }
                } else {
                    row.innerHTML = metrics[metric][index]
                }
                // row.className = 'metrics-line'

                contentCol.append(row)
                // const values = metrics[metric]
            })
            line.append(contentCol)
        }
        planTable.append(line)
    }

    // !! =======================================================================

    plansDiv.append(planTable) // atribui a tabela criada como corpo da div

    return Promise.resolve(dom.serialize().toString())
}

async function handleDashBoard(req, res) {
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
            // Has callback url
            const callback = req.body.callback
            try {
                const url = new URL(callback)
            } catch (err) {
                return response.badRequest('You must pass a valid callback url')
            }
            // Has valid callback url
            response.ok()
            const dashboard = await createDashboardTable()
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

exports.Dashboard = function () {
    return {
        createDashboardTable,
        handleDashBoard
    }
}
