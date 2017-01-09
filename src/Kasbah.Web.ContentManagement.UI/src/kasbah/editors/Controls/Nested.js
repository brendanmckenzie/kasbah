import React from 'react'
import _ from 'lodash'
import NestedForm from './NestedForm'


class Nested extends React.Component {
  static propTypes = {
    input: React.PropTypes.object.isRequired,
    options: React.PropTypes.object
  }

  constructor() {
    super()

    this.state = {
      showModal: false
    }

    this.handleHideModal = this.handleHideModal.bind(this)
    this.handleShowModal = this.handleShowModal.bind(this)
    this.handleSubmit = this.handleSubmit.bind(this)
  }

  static alias = 'nested'

  handleShowModal() {
    this.setState({
      showModal: true
    })
  }

  handleHideModal() {
    this.setState({
      showModal: false
    })
  }

  handleSubmit(values) {
    this.props.input.onChange(values)
    this.handleHideModal()
  }

  get modal() {
    const value = typeof this.props.input.value === 'object' ? this.props.input.value : {}

    return (
      <NestedForm
        type={{ fields: this.props.options.fields }}
        initialValues={value}
        onSubmit={this.handleSubmit}
        onClose={this.handleHideModal} />
    )
  }

  get display() {
    const value = typeof this.props.input.value === 'object' ? this.props.input.value : {}

    return this.props.options.fields.map(fld => (
      <div key={fld.alias}>
        <strong>{fld.displayName}</strong> {value[fld.alias]}
      </div>
    ))
  }

  render() {
    // const { input: { value, onChange } } = this.props

    return (<div>
      {this.display}
      <button type='button' onClick={this.handleShowModal} className='button'>Edit</button>
      {this.state.showModal && this.modal}
    </div>)
  }
}

export default Nested
