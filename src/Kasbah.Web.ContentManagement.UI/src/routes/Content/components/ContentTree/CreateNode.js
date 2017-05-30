import React from 'react'
import CreateNodeForm from 'forms/CreateNodeForm'

class CreateNode extends React.Component {
  static propTypes = {
    visible: React.PropTypes.bool,
    parent: React.PropTypes.string,
    onCancel: React.PropTypes.func,
    createNodeRequest: React.PropTypes.func,
    createNode: React.PropTypes.object,
    listTypesRequest: React.PropTypes.func,
    listTypes: React.PropTypes.object
  }

  constructor() {
    super()

    this.handleSubmit = this.handleSubmit.bind(this)
  }

  componentWillMount() {
    if (this.props.listTypesRequest) {
      this.props.listTypesRequest()
    }
  }

  componentWillReceiveProps(nextProps) {
    if (nextProps.createNode.success && nextProps.createNode.success !== this.props.createNode.success) {
      this.props.onCancel()
    }
  }

  handleSubmit(values) {
    const data = {
      parent: this.props.parent,
      ...values
    }
    this.props.createNodeRequest(data)
  }

  render() {
    if (!this.props.visible) { return null }
    return (
      <CreateNodeForm
        onSubmit={this.handleSubmit}
        onClose={this.props.onCancel}
        types={this.props.listTypes.payload}
        loading={this.props.createNode.loading} />
    )
  }
}

export default CreateNode
