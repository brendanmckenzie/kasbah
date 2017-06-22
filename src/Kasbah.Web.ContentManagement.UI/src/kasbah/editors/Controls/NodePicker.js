import React from 'react'
import PropTypes from 'prop-types'
import moment from 'moment'
import _ from 'lodash'
import { Link } from 'react-router'
import Loading from 'components/Loading'
import { makeApiRequest } from 'store/util'

class NodePicker extends React.Component {
  static propTypes = {
    input: PropTypes.object.isRequired,
    type: PropTypes.string
  }

  static alias = 'nodePicker'

  constructor() {
    super()

    this.state = {
      showModal: false,
      loading: true,
      nodes: [],
      selection: null,
      nodeDetail: {}
    }
  }

  componentWillMount() {
    this.handleReloadNodeDetail(this.props.input.value)
  }

  handleRefresh = () => {
    makeApiRequest({
      url: '/content/nodes/by-type',
      method: 'POST',
      body: {
        type: this.props.type,
        inherit: true
      }
    })
      .then(nodes => {
        this.setState({
          loading: false,
          nodes
        })
      })
  }

  handleShowModal = () => {
    this.handleRefresh()
    this.setState({
      showModal: true
    })
  }

  handleHideModal = () => {
    this.setState({
      showModal: false
    })
  }

  handleSelect = (item) => {
    this.setState({
      selection: item.id
    })
  }

  handleCommit = () => {
    const { input: { onChange } } = this.props

    onChange(this.state.selection)

    this.handleReloadNodeDetail(this.state.selection)

    this.handleHideModal()
  }

  handleReloadNodeDetail = (value) => {
    if (!value) { return }

    makeApiRequest({ url: `/content/node/${value}`, method: 'GET' })
      .then(nodeDetail => {
        this.setState({
          nodeDetail
        })
      })
  }

  handleClear = () => {
    const { input: { onChange } } = this.props

    onChange(null)
    this.setState({
      selection: null
    })
  }

  renderNode(id) {
    if (this.state.nodeDetail) {
      const item = this.state.nodeDetail
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
      <li>
        {this.renderNode(value)}
      </li>
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
          {this.state.loading ? <Loading /> : (
            _(this.state.nodes).sortBy('modified').reverse().value().map(ent => (
              <div key={ent.id} className='control'>
                <div
                  className={'card' + ((this.state.selection === ent.id) ? ' is-selected' : '')}
                  onClick={() => this.handleSelect(ent)}>
                  <div className='card-content'>
                    <p><strong>{ent.displayName}</strong> <small>{ent.alias}</small></p>
                    <p><small>{moment(ent.modified).fromNow()}</small></p>
                  </div>
                </div>
              </div>
            ))
          )}
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
      <div className='level'>
        <div className='level-left' />
        <div className='level-right'>
          <button type='button' className='level-item button is-small' onClick={this.handleClear}>Clear</button>
          <button type='button' className='level-item button is-small' onClick={this.handleShowModal}>Select node</button>
        </div>
      </div>
      {this.modal}
    </div>)
  }
}

export default NodePicker
