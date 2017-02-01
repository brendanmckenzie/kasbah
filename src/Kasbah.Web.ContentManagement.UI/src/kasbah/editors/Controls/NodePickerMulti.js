import React from 'react'
import { Link } from 'react-router'
import Loading from 'components/Loading'
import { makeApiRequest } from 'store/util'

class NodePicker extends React.Component {
  static propTypes = {
    input: React.PropTypes.object.isRequired,
    type: React.PropTypes.string
  }

  static alias = 'nodePickerMulti'

  constructor() {
    super()

    this.state = {
      showModal: false,
      loading: true,
      nodes: [],
      selection: [],
      nodeDetail: {}
    }

    this.handleHideModal = this.handleHideModal.bind(this)
    this.handleShowModal = this.handleShowModal.bind(this)
    this.handleSelect = this.handleSelect.bind(this)
    this.handleCommit = this.handleCommit.bind(this)
  }

  componentWillMount() {
    makeApiRequest({ url: '/content/tree', method: 'GET' })
      .then(nodes => {
        this.setState({
          loading: false,
          nodes: nodes.filter(ent => ent.type.indexOf(this.props.type) !== -1) // TODO: filter this server-side
        })
      })

    this.handleReloadNodeDetail(this.props.input.value)
  }

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

  handleSelect(item) {
    this.setState({
      selection: this.state.selection.indexOf(item) === -1
        ? [...this.state.selection, item]
        : this.state.selection.filter(ent => ent !== item)
    })
  }

  handleCommit() {
    const { input: { onChange } } = this.props

    onChange(this.state.selection)

    this.handleReloadNodeDetail(this.state.selection)

    this.handleHideModal()
  }

  handleReloadNodeDetail(value) {
    if (!value) { return }

    Promise.all(value.map(ent => makeApiRequest({ url: `/content/node/${ent}`, method: 'GET' })))
      .then(res => {
        let detail = {}
        res.forEach(ent => {
          detail[ent.id] = ent
        })
        this.setState({
          nodeDetail: detail
        })
      })
  }

  renderNode(id) {
    if (this.state.nodeDetail[id]) {
      const item = this.state.nodeDetail[id]
      return (
        <div className='level'>
          <div className='level-left'>
            <div className='level-item'>
              <p><Link to={`/content/${id}`}>{item.displayName}</Link></p>
              <p><small>{item.alias}</small></p>
            </div>
          </div>
          <div className='level-right'>
            <span className={`tag ` + (item.publishedVersion ? ' is-primary' : ' is-warning')}>
              {item.publishedVersion ? `v${item.publishedVersion}` : 'not published'}
            </span>
          </div>
        </div>
      )
    } else {
      return (
        <Link to={`/content/${id}`}>{id}</Link>
      )
    }
  }

  get display() {
    const { input: { value } } = this.props

    if (!value) { return null }

    return (<ul>
      {value.map(ent => (
        <li key={ent}>
          {this.renderNode(ent)}
        </li>
      ))}
    </ul>)
  }

  get modal() {
    if (!this.state.showModal) { return null }

    return (<div className='modal is-active'>
      <div className='modal-background' />
      <div className='modal-card'>
        <header className='modal-card-head'>
          <span className='modal-card-title'>
            Node selector
          </span>
        </header>
        <section className='modal-card-body'>
          <div className='columns is-multiline'>
            {this.state.loading ? <Loading /> : (
              this.state.nodes.map(ent => (
                <div key={ent.id} className='column is-4'>
                  <div
                    className={'card' + (this.state.selection.indexOf(ent.id) === -1 ? '' : ' is-selected')}
                    onClick={() => this.handleSelect(ent.id)}>
                    <div className='card-content'>
                      <p><strong>{ent.displayName}</strong></p>
                      <p><small>{ent.alias}</small></p>
                    </div>
                  </div>
                </div>
              ))
            )}
          </div>
        </section>
        <footer className='modal-card-foot'>
          <button type='button' className='button is-primary' onClick={this.handleCommit}>Select</button>
          <button type='button' className='button' onClick={this.handleHideModal}>Cancel</button>
        </footer>
      </div>
    </div>)
  }

  render() {
    return (<div className='node-picker'>
      {this.display}
      <div className='has-text-right'>
        <button type='button' className='button is-small' onClick={this.handleShowModal}>
          Select nodes
        </button>
      </div>
      {this.modal}
    </div>)
  }
}

export default NodePicker
