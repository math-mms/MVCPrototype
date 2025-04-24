const weather_container = $('#weather-list')
const ICON_SRC = '/icons'

$.ajax({
    url: '/Weather',
    type: 'GET',
    success: (response) => {
        for (const weather of response) {
            weather_container[0].appendChild(mountWeatherDisplay(weather))
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
    temperature.classList.add('weather-temperature')
    temperature.innerText = `${weather_object.temperatureC}`
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
