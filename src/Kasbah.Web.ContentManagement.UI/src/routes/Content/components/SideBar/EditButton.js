import React from 'react'
import { connect } from 'react-redux'
import PropTypes from 'prop-types'
import { actions as uiActions } from 'store/appReducers/ui'
import { actions as contentActions } from 'store/appReducers/content'
import NodeForm from 'forms/NodeForm'

class EditButton extends React.Component {
  static propTypes = {
    node: PropTypes.object.isRequired,
    content: PropTypes.object.isRequired,
    showModal: PropTypes.func.isRequired,
    hideModal: PropTypes.func.isRequired,
    putNode: PropTypes.func.isRequired
  }

  handleClick = () => {
    this.props.showModal(null, this.form, null)
  }

  handleSave = (values) => {
    console.log(values)
    this.props.putNode(values)
  }

  get form() {
    return (<NodeForm
      initialValues={this.props.node}
      onClose={this.props.hideModal}
      onSubmit={this.handleSave}
      types={this.props.content.types.list} />)
  }

  render() {
    return (
      <button className='button' type='button' onClick={this.handleClick}>
        <span className='icon is-small'><i className='fa fa-pencil' /></span>
        <span>Edit</span>
      </button>
    )
  }
}

const mapStateToProps = (state, ownProps) => ({
  content: state.content
})

const mapDispatchToProps = {
  ...uiActions,
  ...contentActions
}

export default connect(mapStateToProps, mapDispatchToProps)(EditButton)
