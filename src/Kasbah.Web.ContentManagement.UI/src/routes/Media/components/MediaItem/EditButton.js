import React from 'react'
import { connect } from 'react-redux'
import PropTypes from 'prop-types'
import { actions as uiActions } from 'store/appReducers/ui'
import { actions as mediaActions } from 'store/appReducers/media'
import MediaForm from 'forms/MediaForm'

class EditButton extends React.Component {
  static propTypes = {
    media: PropTypes.object.isRequired,
    className: PropTypes.string,
    showModal: PropTypes.func.isRequired,
    hideModal: PropTypes.func.isRequired,
    putMedia: PropTypes.func.isRequired
  }

  handleClick = () => {
    this.props.showModal(null, this.form, null)
  }

  handleSave = (values) => {
    this.props.putMedia(values)
  }

  get form() {
    return (<MediaForm
      initialValues={this.props.media}
      onClose={this.props.hideModal}
      onSubmit={this.handleSave} />)
  }

  render() {
    return (
      <button className={this.props.className} type='button' onClick={this.handleClick}>
        <span className='icon is-small'><i className='fa fa-pencil' /></span>
        <span>Edit</span>
      </button>
    )
  }
}

const mapDispatchToProps = {
  ...uiActions,
  ...mediaActions
}

export default connect(null, mapDispatchToProps)(EditButton)
