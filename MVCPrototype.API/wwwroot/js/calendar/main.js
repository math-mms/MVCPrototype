const weather_container = $('#weather-list')
const ICON_SRC = '/icons'

$.ajax({
    url: '/Weather',
    type: 'GET',
    success: (response) => {
        const weatherData = checkApiDateCorrespondence(response)
        for (const obj in weatherData) {
            weather_container[0].appendChild(mountWeatherDisplay(weatherData[obj]))
        }
    },
})

function mountWeatherDisplay(weather_object) {
    const weatherTranslations = {
        Freezing: 'Frio',
        Raining: 'Chovendo',
        Smog: 'Nublado',
        Sunny: 'Ensolarado',
    }
    const daysOfWeek = ['Segunda', 'Terça', 'Quarta', 'Quinta', 'Sexta', 'Sábado', 'Domingo']
    const timestamp = Date.parse(weather_object.date)
    const parsedDate = new Date(timestamp)
    const today = daysOfWeek[parsedDate.getDay()]

    const translatedWeather = weatherTranslations[weather_object.summary]

    let weather = document.createElement('DIV')
    weather.classList.add('weather-display')

    let header_title = document.createElement('SPAN')
    header_title.innerText = today
    let header = document.createElement('SPAN')
    header.innerText = weather_object.date

    let footer = document.createElement('SPAN')
    footer.innerText = translatedWeather

    let temperature = document.createElement('SPAN')
    if (weather_object.temperatureC != null) {
        temperature.classList.add('weather-temperature')
        temperature.innerText = `${weather_object.temperatureC}`
    } else {
        temperature.innerText = '??'
    }

    let icon = document.createElement('LABEL')
    icon.appendChild(getIcon(weather_object.summary))

    weather.appendChild(header_title)
    weather.appendChild(header)
    weather.appendChild(temperature)
    weather.appendChild(icon)
    weather.appendChild(footer)

    return weather
}

function getIcon(summary) {
    const icon_img = document.createElement('IMG')
    switch (summary) {
        case 'Raining':
            icon_img.src = ICON_SRC + '/rain-solid.svg'
            break
        case 'Freezing':
            icon_img.src = ICON_SRC + '/snowflake-solid.svg'
            break
        case 'Smog':
            icon_img.src = ICON_SRC + '/smog-solid.svg'
            break
        case 'Sunny':
            icon_img.src = ICON_SRC + '/sun-solid.svg'
            break
    }
    return icon_img
}

function getDaysInCurrentMonth() {
    const today = new Date()
    const year = today.getFullYear()
    const month = today.getMonth() // Mês corrente (0-11)

    // Criar uma lista com os dias do mês
    const daysInMonth = []
    const daysInCurrentMonth = new Date(year, month + 1, 0).getDate() // Pega o último dia do mês

    for (let i = 1; i <= daysInCurrentMonth; i++) {
        const day = new Date(year, month, i)
        daysInMonth.push(day)
    }

    return daysInMonth
}

function checkApiDateCorrespondence(response) {
    // Obter a lista de dias do mês corrente
    const daysInMonth = getDaysInCurrentMonth()

    // Obter o dia da data da API
    const weatherDataForMonth = []

    for (const day of daysInMonth) {
        // Converte para string no formato YYYY-MM-DD
        const dayString = `${day.getFullYear()}-${(day.getMonth() + 1).toString().padStart(2, '0')}-${day
            .getDate()
            .toString()
            .padStart(2, '0')}`

        // Verifica se a data da API corresponde a essa data
        const apiData = response.find((apiDay) => apiDay.date.startsWith(dayString))

        if (apiData) {
            weatherDataForMonth.push({
                date: dayString,
                temperatureC: apiData.temperatureC,
                summary: apiData.summary,
            })
        } else {
            weatherDataForMonth.push({
                date: dayString,
                temperatureC: null,
                summary: null,
            })
        }
    }

    // Agora, `weatherDataForMonth` contém todas as datas do mês com os dados da API (se existirem)
    console.log(weatherDataForMonth)
    return weatherDataForMonth
}
