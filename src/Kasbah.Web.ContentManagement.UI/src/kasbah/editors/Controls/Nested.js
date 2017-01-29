import React from 'react'
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
    const type = {
      displayName: this.props.input.name,
      fields: this.props.options.fields
    }

    return (
      <NestedForm
        type={type}
        initialValues={value}
        onSubmit={this.handleSubmit}
        onClose={this.handleHideModal} />
    )
  }

  get display() {
    const value = typeof this.props.input.value === 'object' ? this.props.input.value : {}

    return (
      <blockquote>
        {this.props.options.fields.map(fld => (
          <div key={fld.alias} className='control'>
            <label className='label'>{fld.displayName}</label>
            <div className='field-value'>{value[fld.alias]}</div>
          </div>
        ))}
      </blockquote >
    )
  }

  render() {
    return (<div className='field-editor__nested'>
      <div className='has-text-right'>
        <button type='button' className='button is-small' onClick={this.handleShowModal}>
          Edit {this.props.input.name}
        </button>
      </div>
      {this.display}
      <div className='has-text-right'>
        <button type='button' className='button is-small' onClick={this.handleShowModal}>
          Edit {this.props.input.name}
        </button>
      </div>
      {this.state.showModal && this.modal}
    </div>)
  }
}

export default Nested
