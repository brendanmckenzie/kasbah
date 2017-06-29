import React from 'react'
import PropTypes from 'prop-types'
import moment from 'moment'
import ItemButton from 'components/ItemButton'
// import UserForm from 'forms/UserForm'

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
        <td>
          <ItemButton
            type='button' className='button is-small'
            onClick={this.handleShowModal} item={user}>Edit</ItemButton>
        </td>
      </tr>
    )
  }
}

export default UserListRow
