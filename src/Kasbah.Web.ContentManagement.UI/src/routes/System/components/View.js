import React from 'react'
import moment from 'moment'

class View extends React.Component {
  componentWillMount() {
    this.props.listInstancesRequest()
  }

  render() {
    return (
      <div className='section'>
        <div className='container'>
          <h1 className='title'>System</h1>

          <div className='columns'>
            <div className='column has-text-centered'>
              <p className='heading'>System start date</p>
              <p className='title'>Sun 8 Jan</p>
            </div>
            <div className='column has-text-centered'>
              <p className='heading' title='In the past 24 hours'>Uptime</p>
              <p className='title'>4 days 12 hours</p>
            </div>
            <div className='column has-text-centered'>
              <p className='heading'>Requests served</p>
              <p className='title'>1,200,234</p>
            </div>
            <div className='column has-text-centered'>
              <p className='heading'>Requests per second</p>
              <p className='title'>0.24</p>
            </div>
          </div>

          <div className='columns'>
            <div className='column'>
              system logs go here
            </div>
            <div className='column'>
              tables and stuff go here
            </div>
          </div>

          <h2 className='subtitle'>Running instances</h2>
          {this.props.listInstances.success && (<table className='table'>
            <thead>
              <tr>
                <th>Id</th>
                <th>Started</th>
                <th>Uptime</th>
                <th>Heartbeat</th>
                <th>Total Requests</th>
                <th>Recent requests</th>
              </tr>
            </thead>
            <tbody>
              {this.props.listInstances.payload.map(ent => (
                <tr>
                  <td>{ent.id}</td>
                  <td>{moment.utc(ent.started).calendar()}</td>
                  <td>{moment.utc(ent.started).fromNow(true)}</td>
                  <td>{moment.utc(ent.heartbeat).fromNow()}</td>
                  <td>{ent.requestsTotal}</td>
                  <td>{ent.requestsLatest}</td>
                </tr>
              ))}
            </tbody>
          </table>)}
        </div>
      </div>
    )
  }
}

export default View
