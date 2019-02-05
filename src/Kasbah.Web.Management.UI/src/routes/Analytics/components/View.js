import React from 'react'
import { Line } from 'react-chartjs-2'
import moment from 'moment'
import { Section, Container } from 'components/Layout'
import { makeApiRequest } from 'store/util'

class View extends React.Component {
  state = {
    session: null,
    sessionActivity: null,
    sessions: null,
    sessionDetail: null,
    start: moment().add(-2, 'day').format('YYYY-MM-DD'),
    end: moment().add(1, 'hour').format('YYYY-MM-DD'),
    interval: 'hour'
  }

  componentDidMount() {
    makeApiRequest({
      method: 'GET',
      url: `/analytics/reporting/session?interval=${this.state.interval}&start=${this.state.start}&end=${this.state.end}`
    })
      .then(res => {
        this.setState({ session: res })
      })

    makeApiRequest({
      method: 'GET',
      url: `/analytics/reporting/session-activity?type=view&interval=${this.state.interval}&start=${this.state.start}&end=${this.state.end}`
    })
      .then(res => {
        this.setState({ sessionActivity: res })
      })

    makeApiRequest({
      method: 'GET',
      url: `/analytics/reporting/sessions?skip=0&take=10`
    })
      .then(res => {
        this.setState({ sessions: res })
      })
  }

  loadDetail = (id) => {
    makeApiRequest({
      method: 'GET',
      url: `/analytics/reporting/sessions/${id}?skip=0&take=20`
    })
      .then(res => {
        this.setState({ sessionDetail: res })
      })
  }

  get chart() {
    if (!this.state.session) {
      return null
    }

    const start = moment(this.state.start)
    const end = moment(this.state.end)
    const diff = end.diff(start, this.state.interval)

    const labels = Array.from({ length: diff })
      .map((_, index) => moment(start).add(index, this.state.interval))

    const chartData = {
      labels: labels.map(ent => ent.format('DD MMM H:mm')),
      datasets: [{
        label: 'Sessions',
        data: labels.map(dt => {
          const data = this.state.session.find(ent => moment.utc(ent.period).isSame(dt, this.state.interval))
          return data ? data.count : 0
        }),
        fill: false,
        borderColor: 'rgb(114,147,203)'
      },
      {
        label: 'Session activity (page views)',
        data: labels.map(dt => {
          const data = this.state.sessionActivity.find(ent => moment.utc(ent.period).isSame(dt, this.state.interval))
          return data ? data.count : 0
        }),
        fill: false,
        borderColor: 'rgb(225,151,76)'
      }]
    }

    const chartOptions = {
      animated: false,
      animation: {
        duration: 0,
      },
      hover: {
        animationDuration: 0,
      },
      responsiveAnimationDuration: 0,
    }

    return <Line data={chartData} options={chartOptions} />
  }

  get table() {
    if (!this.state.sessions) { return null }

    return (<table className='table is-hoverable is-fullwidth'>
      <thead>
        <tr>
          <th>Name</th>
          <th style={{ width: 200 }}>Created</th>
          <th style={{ width: 200 }}>Last activity</th>
        </tr>
      </thead>
      <tbody>
        {this.state.sessions.map(ent => <tr key={ent.id}>
          <td>
            <button
              className='button'
              onClick={() => this.loadDetail(ent.id)}>{(ent.attributes && ent.attributes['name']) || 'Unknown'}</button>
          </td>
          <td>{moment(ent.created).format('DD MMM H:mm')}</td>
          <td>{moment.utc(ent.lastActivity).fromNow()}</td>
        </tr>)}
      </tbody>
    </table>)
  }

  get detail() {
    if (!this.state.sessionDetail) { return null }

    return (<table className='table is-hoverable is-fullwidth'>
      <thead>
        <tr>
          <th>Type</th>
          <th>Url</th>
          <th style={{ width: 200 }}>Created</th>
          <th>Detail</th>
        </tr>
      </thead>
      <tbody>
        {this.state.sessionDetail.map(ent => <tr key={ent.id}>
          <td>{ent.type}</td>
          <td>{ent.attributes ? ent.attributes.url : null}</td>
          <td>{moment.utc(ent.created).fromNow(true)}</td>
          <td><pre>{JSON.stringify(ent.attributes, null, 2)}</pre></td>
        </tr>)}
      </tbody>
    </table>)
  }

  render() {
    return (
      <Section>
        <Container>
          {this.chart}
          {this.table}
          {this.detail}
        </Container>
      </Section>
    )
  }
}

export default View
