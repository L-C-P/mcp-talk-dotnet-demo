import { defineAppSetup } from '@slidev/types'

export default defineAppSetup(({ router }) => {

  const getPresenterSlideId = (path: string) => {
    const match = path.match(/\/presenter\/([^/?#]+)/)
    return match?.[1]
  }
  const clickPresenterTimerPlay = () => {
    if (typeof document === 'undefined')
      return false

    const playIcon = document.querySelector<HTMLElement>('.slidev-presenter .grid-section.bottom .i-carbon\\:play')
    const toggleButton = playIcon?.parentElement as HTMLElement | null
    if (!toggleButton)
      return false

    toggleButton.click()
    return true
  }

  let timerAutoStartRunId = 0

  const schedulePresenterTimerAutoStart = () => {
    timerAutoStartRunId += 1
    const runId = timerAutoStartRunId
    const maxAttempts = 6
    const retryDelayMs = 250

    const attemptAutoStart = (attempt: number) => {
      if (runId !== timerAutoStartRunId)
        return

      const currentPresenterSlide = getPresenterSlideId(router.currentRoute.value.path)
      if (!currentPresenterSlide)
        return

      clickPresenterTimerPlay()

      if (attempt >= maxAttempts)
        return

      setTimeout(() => {
        requestAnimationFrame(() => {
          attemptAutoStart(attempt + 1)
        })
      }, retryDelayMs)
    }
    requestAnimationFrame(() => {
      if (runId !== timerAutoStartRunId)
        return
      attemptAutoStart(1)
    })
  }

  router.afterEach((to, from) => {
    const toSlide = getPresenterSlideId(to.path)
    const fromSlide = getPresenterSlideId(from.path)
    if (!toSlide || !fromSlide || toSlide === fromSlide)
      return

    schedulePresenterTimerAutoStart()
  })
})
