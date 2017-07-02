import React from 'react'
import PropTypes from 'prop-types'
import moment from 'moment'
import EditButton from './EditButton'

class UserListRow extends React.Component {
  static propTypes = {
    user: PropTypes.object.isRequired
  }

  render() {
    const { user } = this.props
    return (
      <tr>
        <td>{user.name}</td>
        <td>{user.username}</td>
        <td>{user.email}</td>
        <td>{moment.utc(user.lastLogin).fromNow()}</td>
        <td><EditButton className='button is-small is-secondary' user={user} /></td>
      </tr>
    )
  }
}

export default UserListRow
