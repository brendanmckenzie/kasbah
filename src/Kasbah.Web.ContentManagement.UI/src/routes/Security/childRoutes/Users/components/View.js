import React from 'react'
import PropTypes from 'prop-types'
import moment from 'moment'
import Loading from 'components/Loading'
import ItemButton from 'components/ItemButton'
import UserForm from 'forms/UserForm'

class View extends React.Component {
  static propTypes = {
    listUsersRequest: PropTypes.func.isRequired,
    listUsers: PropTypes.object.isRequired,
    createUserRequest: PropTypes.func.isRequired,
    createUser: PropTypes.object.isRequired,
    putUserRequest: PropTypes.func.isRequired,
    putUser: PropTypes.object.isRequired
  }

  constructor() {
    super()

    this.state = { showModal: false }
  }

  componentWillMount() {
    this.handleRefresh()
  }

  componentWillReceiveProps(nextProps) {
    if (nextProps.createUser.success && (nextProps.createUser.loading !== this.props.createUser.loading)) {
      this.handleRefresh()
      this.handleHideModal()
    }

    if (nextProps.putUser.success && (nextProps.putUser.loading !== this.props.putUser.loading)) {
      this.handleRefresh()
      this.handleHideModal()
    }
  }

  handleRefresh = () => {
    this.props.listUsersRequest()
  }

  handleShowModal = (editing) => {
    this.setState({
      showModal: true,
      editing: editing.id ? editing : null
    })
  }

  handleHideModal = () => {
    this.setState({
      showModal: false,
      editing: null
    })
  }

  handleSave = (values) => {
    if (this.state.editing) {
      this.props.putUserRequest(values)
    } else {
      this.props.createUserRequest(values)
    }
  }

  get modal() {
    if (!this.state.showModal) { return null }

    return (<UserForm
      onClose={this.handleHideModal}
      onSubmit={this.handleSave}
      initialValues={this.state.editing}
      loading={this.props.createUser.loading || this.props.putUser.loading} />)
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
          <tr key={ent.id}>
            <td>{ent.name}</td>
            <td>{ent.username}</td>
            <td>{ent.email}</td>
            <td>{moment.utc(ent.lastLogin).fromNow()}</td>
            <td>
              <ItemButton
                type='button' className='button is-small'
                onClick={this.handleShowModal} item={ent}>Edit</ItemButton>
            </td>
          </tr>
        ))}
      </tbody>
    </table>
    )
  }

  render() {
    return (
      <div>
        <div className='level'>
          <div className='level-left'>
            <h1 className='level-item subtitle'>Users</h1>
          </div>
          <div className='level-right'>
            <button
              type='button'
              className='level-item button is-primary'
              onClick={this.handleShowModal}>New user</button>
          </div>
        </div>
        {this.table}
        {this.modal}
      </div>
    )
  }
}

export default View
