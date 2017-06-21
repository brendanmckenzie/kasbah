import React from 'react'
import PropTypes from 'prop-types'
import moment from 'moment'
import Loading from 'components/Loading'

class View extends React.Component {
  static propTypes = {
    listUsersRequest: PropTypes.func.isRequired,
    listUsers: PropTypes.object.isRequired
  }

  componentWillMount() {
    this.props.listUsersRequest()
  }

  get table() {
    if (this.props.listUsers.loading) {
      return <Loading />
    }

    return (<table className='table is-hover'>
      <thead>
        <tr>
          <th>Name</th>
          <th style={{ width: 200 }}>Username</th>
          <th style={{ width: 200 }}>Email</th>
          <th style={{ width: 200 }}>Last login</th>
          <th />
        </tr>
      </thead>
      <tbody>
        {this.props.listUsers.payload.map(ent => (
          <tr>
            <td>{ent.name}</td>
            <td>{ent.username}</td>
            <td>{ent.email}</td>
            <td>{moment.utc(ent.lastLogin).fromNow()}</td>
          </tr>
        ))}
      </tbody>
    </table>
    )
  }

  render() {
    return (
      <div>
        {this.table}
      </div>
    )
  }
}

export default View
